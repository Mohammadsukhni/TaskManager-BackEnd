using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using TaskManager.Core.Exceptions;
using TaskManager.Core.IService;

namespace TaskManager.Infrastructure.Service
{
    public class EmailServices : IEmailService
    {
        private const string EmailConfigPrefix = "Email";
        private readonly IConfiguration _configuration;

        public EmailServices(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var settings = GetEmailSettings();
            var secureSocketOption = settings.UseSsl
                ? SecureSocketOptions.SslOnConnect
                : SecureSocketOptions.StartTls;

            var email = new MimeMessage();

            email.From.Add(MailboxAddress.Parse(settings.From));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html")
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(settings.Host, settings.Port, secureSocketOption);
            await smtp.AuthenticateAsync(settings.UserName, settings.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }

        private EmailSettings GetEmailSettings()
        {
            return new EmailSettings(
                GetRequiredSetting("From"),
                GetRequiredSetting("SmtpUserName"),
                GetRequiredSetting("SmtpPassword"),
                GetRequiredSetting("SmtpHost"),
                GetPort(),
                GetUseSsl());
        }

        private string GetRequiredSetting(string key)
        {
            return _configuration[$"{EmailConfigPrefix}:{key}"] ??
                   throw new ConfigurationException($"{EmailConfigPrefix}:{key} is missing.");
        }

        private int GetPort()
        {
            var portText = GetRequiredSetting("SmtpPort");

            if (!int.TryParse(portText, out var port))
                throw new ConfigurationException("Email:SmtpPort must be a valid port number.");

            return port;
        }

        private bool GetUseSsl()
        {
            var useSslText = _configuration["Email:UseSsl"] ?? "false";

            if (!bool.TryParse(useSslText, out var useSsl))
                throw new ConfigurationException("Email:UseSsl must be true or false.");

            return useSsl;
        }

        private sealed record EmailSettings(
            string From,
            string UserName,
            string Password,
            string Host,
            int Port,
            bool UseSsl);
    }
}
