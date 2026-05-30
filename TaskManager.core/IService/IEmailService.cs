using System;
using System.Collections.Generic;
using System.Text;

namespace TaskManager.Core.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string body);
    }
}
