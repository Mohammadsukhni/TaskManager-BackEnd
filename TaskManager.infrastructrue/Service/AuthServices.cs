using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Core.Constants;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Exceptions;
using TaskManager.Core.Helper;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class AuthServices : IAuthServices
    {
        private const int OtpLifetimeInMinutes = 5;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly ILoginAttemptService _loginAttemptService;

        public AuthServices(
            IUnitOfWork unitOfWork,
            IConfiguration configuration,
            IEmailService emailService,
            ILoginAttemptService loginAttemptService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
            _loginAttemptService = loginAttemptService;
        }

        public async Task LoginAsync(LoginDto dto)
        {
            _loginAttemptService.EnsureLoginAllowed(dto.Email);

            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
            {
                _loginAttemptService.RegisterFailedAttempt(dto.Email);
                throw new BadRequestException("Email is incorrect.");
            }

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated. You cannot login.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _loginAttemptService.RegisterFailedAttempt(dto.Email);
                throw new BadRequestException("Password is incorrect.");
            }

            _loginAttemptService.ResetFailedAttempts(dto.Email);

            var otpCode = OtpHelper.GenerateOtp();

            await CreateOtpAsync(user.Id, otpCode, OtpActionType.Login);
            await SendLoginOtpAsync(user.Email, otpCode);
        }

        public async Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                return null;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated. You cannot login.");

            var otp = await GetValidOtpAsync(user.Id, dto.Otp, OtpActionType.Login);

            if (otp == null)
                return null;

            await _unitOfWork.Otps.Delete(otp);

            return GenerateJwtToken(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                throw new BadRequestException("Email is incorrect.");

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated.");

            var otpCode = OtpHelper.GenerateOtp();

            await CreateOtpAsync(user.Id, otpCode, OtpActionType.ForgetPassword);
            await SendResetPasswordOtpAsync(user.Email, otpCode);
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                throw new BadRequestException("Email is incorrect.");

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated.");

            var otp = await GetValidOtpAsync(user.Id, dto.Otp, OtpActionType.ForgetPassword);

            if (otp == null)
                throw new BadRequestException("Invalid OTP.");

            if (dto.NewPassword != dto.ConfirmPassword)
                throw new BadRequestException("Passwords do not match.");

            PasswordPolicyHelper.Validate(dto.NewPassword);

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _unitOfWork.Otps.Delete(otp);
            await _unitOfWork.Users.Update(user);
        }

        public Task ValidatePasswordPolicyAsync(ValidatePasswordPolicyDto dto)
        {
            PasswordPolicyHelper.Validate(dto.Password);

            return Task.CompletedTask;
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            var trimmedEmail = email.Trim();
            var normalizedEmail = trimmedEmail.ToLower();
            var users = await _unitOfWork.Users.GetAllAsync(x => x.Email.ToLower() == normalizedEmail);

            return users.FirstOrDefault(x =>
                string.Equals(x.Email, trimmedEmail, StringComparison.OrdinalIgnoreCase));
        }

        private async Task EnsureAdminActiveAsync(User user)
        {
            if (user.UserRole != UserRole.Admin || user.IsActive)
                return;

            user.IsActive = true;
            await _unitOfWork.Users.Update(user);
        }

        private async Task CreateOtpAsync(int receiverId, string code, OtpActionType actionType)
        {
            await DeleteExistingOtpsAsync(receiverId, actionType);

            var otp = new Otp
            {
                ReceiverId = receiverId,
                Code = code,
                ActionType = actionType
            };

            await _unitOfWork.Otps.CreateAsync(otp);
        }

        private async Task<Otp?> GetValidOtpAsync(int receiverId, string code, OtpActionType actionType)
        {
            var otps = await _unitOfWork.Otps.GetAllAsync(
                x => x.ReceiverId == receiverId &&
                     x.ActionType == actionType &&
                     x.Code == code);

            return otps
                .Where(x => x.CreatedDate.AddMinutes(OtpLifetimeInMinutes) >= DateTime.Now)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();
        }

        private async Task DeleteExistingOtpsAsync(int receiverId, OtpActionType actionType)
        {
            var otps = await _unitOfWork.Otps.GetAllAsync(
                x => x.ReceiverId == receiverId &&
                     x.ActionType == actionType);

            foreach (var otp in otps)
            {
                await _unitOfWork.Otps.Delete(otp);
            }
        }

        private async Task SendLoginOtpAsync(string email, string otpCode)
        {
            await _emailService.SendEmailAsync(
                email,
                "Task Manager OTP",
                $"<h3>Your OTP Code is: {otpCode}</h3>");
        }

        private async Task SendResetPasswordOtpAsync(string email, string otpCode)
        {
            await _emailService.SendEmailAsync(
                email,
                "Reset Password OTP",
                $"<h3>Your Reset Password OTP is: {otpCode}</h3>");
        }

        private AuthResponseDto GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, ApplicationRoles.FromUserRole(user.UserRole))
            };

            var jwtKey = _configuration["Jwt:Key"] ??
                         throw new ConfigurationException("Jwt:Key is missing.");

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtKey));

            var credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256);

            var duration = _configuration["Jwt:DurationInMinutes"] ??
                           _configuration["Jwt:ExpireMinutes"] ??
                           "60";

            var expiration = DateTime.UtcNow.AddMinutes(Convert.ToDouble(duration));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: expiration,
                signingCredentials: credentials);

            return new AuthResponseDto
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };
        }
    }
}
