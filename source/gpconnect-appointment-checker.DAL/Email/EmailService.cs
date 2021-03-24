using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Net.Mail;

namespace gpconnect_appointment_checker.DAL.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IAuditService _auditService;
        private readonly IHttpContextAccessor _context;
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration, ILogger<EmailService> logger, IAuditService auditService, IHttpContextAccessor context)
        {
            _logger = logger;
            _auditService = auditService;
            _context = context;
            _configuration = configuration;
            _smtpClient = smtpClient;
        }

        public void SendAuthorisationEmail(string recipient)
        {
            var body = GetEmailTemplate(MailTemplate.AuthorisedConfirmationEmail);
            if (!string.IsNullOrEmpty(body))
            {
                var mailMessage = new MailMessage
                {
                    Subject = "",
                    Body = body,
                    To = {recipient}
                };
                SendEmail(mailMessage);
            }
        }

        private void SendEmail(MailMessage mailMessage)
        {
            try
            {
                var sender = _configuration.GetSection("Email:sender_address").GetConfigurationString();
                mailMessage.From = new MailAddress(sender);
                mailMessage.IsBodyHtml = false;
                _smtpClient.Send(mailMessage);
            }
            catch (Exception exc)
            {
                _logger?.LogError(exc, "An error has occurred while attempting to send an email");
                throw;
            }
        }

        private string GetEmailTemplate(MailTemplate mailTemplate)
        {
            var path = $"{AppContext.BaseDirectory}\\Email\\Templates\\{mailTemplate}.txt";
            if (File.Exists(path))
            {
                var readText = File.ReadAllText(path);
                return readText;
            }
            return null;
        }
    }
}
