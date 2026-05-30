using Microsoft.AspNetCore.Mvc;
using TaskManager.Core.Dto;
using TaskManager.Core.IService;

namespace TaskManager_p.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthsController : ControllerBase
    {
        private readonly IAuthServices _authServices;

        public AuthsController(IAuthServices authServices)
        {
            _authServices = authServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            try
            {
                await _authServices.LoginAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("OTP sent successfully");
        }

        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp(VerifyOtpDto dto)
        {
            AuthResponseDto? result;

            try
            {
                result = await _authServices.VerifyOtpAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            if (result == null)
                return BadRequest("Invalid OTP");

            return Ok(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto)
        {
            try
            {
                await _authServices.ForgotPasswordAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("OTP sent successfully");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
        {
            try
            {
                await _authServices.ResetPasswordAsync(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Password reset successfully");
        }
    }
}
