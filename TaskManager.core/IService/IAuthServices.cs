using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface IAuthServices
    {
        Task LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);
        Task ValidatePasswordPolicyAsync(ValidatePasswordPolicyDto dto);
    }
}
