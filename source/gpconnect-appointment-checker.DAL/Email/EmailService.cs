using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Audit;
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
        private readonly MailAddress _emailSender;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration, ILogger<EmailService> logger, IAuditService auditService, IHttpContextAccessor context)
        {
            _logger = logger;
            _auditService = auditService;
            _context = context;
            _configuration = configuration;
            _smtpClient = smtpClient;
            _emailSender = new MailAddress(_configuration.GetSection("Email:sender_address").GetConfigurationString());
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
                //SendEmail(mailMessage);
                SendToAudit(recipient, body);
            }
        }

        private void SendEmail(MailMessage mailMessage)
        {
            try
            {
                mailMessage.From = _emailSender;
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

        private void SendToAudit(string recipient, string details)
        {
            var auditEntry = new Entry
            {
                Item1 = _emailSender.Address,
                Item2 = recipient,
                Details = details,
                EntryTypeId = (int)AuditEntryType.EmailSent
            };
            _auditService.AddEntry(auditEntry);
        }
    }
}
