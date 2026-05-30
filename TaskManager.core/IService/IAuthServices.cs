using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Dto;

namespace TaskManager.Core.IService
{
    public interface IAuthServices
    {
        Task LoginAsync(LoginDto dto);
        Task<AuthResponseDto?> VerifyOtpAsync(VerifyOtpDto dto);
        Task ForgotPasswordAsync(ForgotPasswordDto dto);
        Task ResetPasswordAsync(ResetPasswordDto dto);

    }
}
