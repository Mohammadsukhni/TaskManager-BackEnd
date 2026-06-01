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

        public AuthServices(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService, ILoginAttemptService loginAttemptService)
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
                throw new BadRequestException("Invalid email or password.");
            }

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated. You cannot login.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            {
                _loginAttemptService.RegisterFailedAttempt(dto.Email);
                throw new BadRequestException("Invalid email or password.");
            }

            _loginAttemptService.ResetFailedAttempts(dto.Email);

            var otpCode = OtpHelper.GenerateOtp();

            await CreateOtpAsync(user.Id, otpCode, OtpActionType.Login);

            await _emailService.SendEmailAsync(
                user.Email,
                "Task Manager OTP",
                $"<h3>Your OTP Code is: {otpCode}</h3>");
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
                return;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated.");

            var otpCode = OtpHelper.GenerateOtp();

            await CreateOtpAsync(user.Id, otpCode, OtpActionType.ForgetPassword);

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password OTP",
                $"<h3>Your Reset Password OTP is: {otpCode}</h3>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                return;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new BadRequestException("This account is deactivated.");

            var otp = await GetValidOtpAsync(user.Id, dto.Otp, OtpActionType.ForgetPassword);

            if (otp == null)
                return;

            if (dto.NewPassword != dto.ConfirmPassword)
                return;

            PasswordPolicyHelper.Validate(dto.NewPassword);

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            await _unitOfWork.Otps.Delete(otp);
            await _unitOfWork.Users.Update(user);
        }

        private async Task<User?> GetUserByEmailAsync(string email)
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.FirstOrDefault(x => x.Email == email);
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
            var otps = await _unitOfWork.Otps.GetAllAsync();

            return otps
                .Where(x => x.ReceiverId == receiverId &&
                            x.ActionType == actionType &&
                            x.Code == code &&
                            x.CreatedDate.AddMinutes(OtpLifetimeInMinutes) >= DateTime.Now)
                .OrderByDescending(x => x.CreatedDate)
                .FirstOrDefault();
        }

        private async Task DeleteExistingOtpsAsync(int receiverId, OtpActionType actionType)
        {
            var otps = await _unitOfWork.Otps.GetAllAsync();

            foreach (var otp in otps.Where(x => x.ReceiverId == receiverId &&
                                                x.ActionType == actionType))
            {
                await _unitOfWork.Otps.Delete(otp);
            }
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
