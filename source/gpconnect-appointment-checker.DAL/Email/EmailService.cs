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
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SmtpClient _smtpClient;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration, ILogger<EmailService> logger, IAuditService auditService, IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _auditService = auditService;
            _configuration = configuration;
            _smtpClient = smtpClient;
            _contextAccessor = contextAccessor;
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
            var sender = _configuration.GetSection("Email:sender_address").GetConfigurationString(null, true);
            var displayName = _configuration.GetSection("General:product_name").GetConfigurationString(sender);
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(sender, displayName),
                    IsBodyHtml = false,
                    Subject = _configuration.GetSection("Email:default_subject").GetConfigurationString(),
                    Body = body,
                    To = { recipient },
                    Bcc = { sender }
                };
                _smtpClient.Send(mailMessage);
                SendToAudit(recipient, body);
            }
            catch (ArgumentNullException argumentNullException)
            {
                _logger?.LogError(argumentNullException, "One of the required arguments for sending an email is empty");
                throw;
            }
            catch (SmtpException smtpException)
            {
                _logger?.LogError(smtpException, "An SMTP error has occurred while attempting to send an email");
                throw;
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "A general error has occurred while attempting to send an email");
                throw;
            }
        }

        private string GetEmailTemplate(MailTemplate mailTemplate)
        {
            var file = FileHelper.ReadFileContents($@"Email\Templates\{mailTemplate}.txt");
            if(file != null)
            {
                return PopulateDynamicFields(file);
            }
            return null;
        }

        private string PopulateDynamicFields(string readText)
        {
            readText = readText.Replace("<address>", _configuration.GetSection("General:get_access_email_address").GetConfigurationString(string.Empty));
            readText = readText.Replace("<url>", _contextAccessor.HttpContext.GetBaseSiteUrl());
            return readText;
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
