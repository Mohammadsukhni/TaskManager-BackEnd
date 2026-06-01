using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    public class AuthsController : BaseController
    {
        private readonly IAuthServices _authServices;

        public AuthsController(
            IAuthServices authServices,
            ICurrentUserService currentUserService)
            : base(currentUserService)
        {
            _authServices = authServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            await _authServices.LoginAsync(dto);

            return Ok("OTP sent successfully");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            var result = await _authServices.VerifyOtpAsync(dto);

            if (result == null)
                return BadRequest("Invalid OTP");

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            await _authServices.ForgotPasswordAsync(dto);

            return Ok("OTP sent successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            await _authServices.ResetPasswordAsync(dto);

            return Ok("Password reset successfully");
        }

        [HttpPost("validate-password-policy")]
        public async Task<IActionResult> ValidatePasswordPolicy(ValidatePasswordPolicyDto dto)
        {
            await _authServices.ValidatePasswordPolicyAsync(dto);

            return Ok("Password policy is valid");
        }
    }
}
