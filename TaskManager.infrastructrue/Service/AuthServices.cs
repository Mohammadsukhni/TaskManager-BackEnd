using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManager.Core.Dto;
using TaskManager.Core.Entities;
using TaskManager.Core.Enum;
using TaskManager.Core.Helper;
using TaskManager.Core.IRepositories;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class AuthServices : IAuthServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;

        public AuthServices(IUnitOfWork unitOfWork, IConfiguration configuration, IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
            _emailService = emailService;
        }

        public async Task LoginAsync(LoginDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                throw new InvalidOperationException("Invalid email or password.");

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new InvalidOperationException("This account is deactivated. You cannot login.");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                throw new InvalidOperationException("Invalid email or password.");

            user.OtpCode = OtpHelper.GenerateOtp();
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(5);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Task Manager OTP",
                $"<h3>Your OTP Code is: {user.OtpCode}</h3>");
        }

        public async Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                return null;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new InvalidOperationException("This account is deactivated. You cannot login.");

            if (user.OtpCode != dto.Otp)
                return null;

            if (user.OtpExpiration == null ||
                user.OtpExpiration < DateTime.UtcNow)
                return null;

            user.OtpCode = null;
            user.OtpExpiration = null;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return GenerateJwtToken(user);
        }

        public async Task ForgotPasswordAsync(ForgotPasswordDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                return;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new InvalidOperationException("This account is deactivated.");

            user.OtpCode = OtpHelper.GenerateOtp();
            user.OtpExpiration = DateTime.UtcNow.AddMinutes(5);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendEmailAsync(
                user.Email,
                "Reset Password OTP",
                $"<h3>Your Reset Password OTP is: {user.OtpCode}</h3>");
        }

        public async Task ResetPasswordAsync(ResetPasswordDto dto)
        {
            var user = await GetUserByEmailAsync(dto.Email);

            if (user == null)
                return;

            await EnsureAdminActiveAsync(user);

            if (!user.IsActive)
                throw new InvalidOperationException("This account is deactivated.");

            if (user.OtpCode != dto.Otp)
                return;

            if (user.OtpExpiration == null ||
                user.OtpExpiration < DateTime.UtcNow)
                return;

            if (dto.NewPassword != dto.ConfirmPassword)
                return;

            PasswordPolicyHelper.Validate(dto.NewPassword);

            user.PasswordHash =
                BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            user.OtpCode = null;
            user.OtpExpiration = null;

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
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
            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();
        }

        private AuthResponseDto GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.UserRole.ToString())
            };

            var jwtKey = _configuration["Jwt:Key"] ??
                         throw new InvalidOperationException("Jwt:Key is missing.");

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
