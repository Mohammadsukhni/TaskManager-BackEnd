using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;
using TaskManager.Core.Exceptions;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class EmailServices : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var from = _configuration["Email:From"] ??
                       throw new ConfigurationException("Email:From is missing.");

            var userName = _configuration["Email:SmtpUserName"] ??
                           throw new ConfigurationException("Email:SmtpUserName is missing.");

            var password = _configuration["Email:SmtpPassword"] ??
                           throw new ConfigurationException("Email:SmtpPassword is missing.");

            var host = _configuration["Email:SmtpHost"] ??
                       throw new ConfigurationException("Email:SmtpHost is missing.");

            var port = _configuration["Email:SmtpPort"] ??
                       throw new ConfigurationException("Email:SmtpPort is missing.");

            var useSslText = _configuration["Email:UseSsl"] ?? "false";
            var useSsl = bool.Parse(useSslText);

            var secureSocketOption = useSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(from));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;

            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                host,
                int.Parse(port),
                secureSocketOption);

            await smtp.AuthenticateAsync(
                userName,
                password);

            await smtp.SendAsync(email);

            await smtp.DisconnectAsync(true);
        }
    }
}
