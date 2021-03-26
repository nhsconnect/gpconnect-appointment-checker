using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Audit;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
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
        private readonly IConfiguration _configuration;
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration, ILogger<EmailService> logger, IAuditService auditService)
        {
            _logger = logger;
            _auditService = auditService;
            _configuration = configuration;
            _smtpClient = smtpClient;
        }

        public void SendUserStatusEmail(bool isAuthorised, string recipient)
        {
            var template = isAuthorised
                ? MailTemplate.AuthorisedConfirmationEmail
                : MailTemplate.DeauthorisedConfirmationEmail;
            var body = GetEmailTemplate(template);
            SendEmail(recipient, body);
        }

        private void SendEmail(string recipient, string body)
        {
            if (string.IsNullOrEmpty(recipient)) throw new ArgumentNullException(nameof(recipient));
            if (string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration.GetSection("Email:sender_address").GetConfigurationString()),
                    IsBodyHtml = false,
                    Subject = _configuration.GetSection("Email:default_subject").GetConfigurationString(),
                    Body = body,
                    To = { recipient }
                };
                //_smtpClient.Send(mailMessage);
                SendToAudit(recipient, body);
            }
            catch (ArgumentNullException argumentNullException)
            {
                _logger?.LogError(argumentNullException, "One of the required arguments for sending an email is empty");
                throw;
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

        private void SendToAudit(string recipient, string details)
        {
            var auditEntry = new Entry
            {
                Item1 = _configuration.GetSection("Email:sender_address").GetConfigurationString(),
                Item2 = recipient,
                Details = details,
                EntryTypeId = (int)AuditEntryType.EmailSent
            };
            _auditService.AddEntry(auditEntry);
        }
    }
}
