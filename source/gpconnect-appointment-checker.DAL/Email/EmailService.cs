using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Audit;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace gpconnect_appointment_checker.DAL.Email
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IAuditService _auditService;
        private readonly IDataService _dataService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SmtpClient _smtpClient;
        private readonly Lazy<List<EmailTemplate>> _emailTemplates;

        public EmailService(SmtpClient smtpClient, IConfiguration configuration, ILogger<EmailService> logger, IAuditService auditService, IHttpContextAccessor contextAccessor, IDataService dataService)
        {
            _logger = logger;
            _auditService = auditService;
            _configuration = configuration;
            _smtpClient = smtpClient;
            _contextAccessor = contextAccessor;
            _dataService = dataService;
            _emailTemplates = new Lazy<List<EmailTemplate>>(GetEmailTemplates);
        }

        public void SendUserStatusEmail(UserAccountStatus userAccountStatus, string recipient)
        {
            EmailTemplate template = null;
            switch(userAccountStatus)
            {
                case UserAccountStatus.Authorised:
                    template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.AuthorisedConfirmationEmail);
                    break;
                case UserAccountStatus.Deauthorised:
                    template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.DeauthorisedConfirmationEmail);
                    break;
            }

            if (template != null)           
            {
                SendEmail(recipient, template);
            }
        }

        public void SendUserCreateAccountEmail(DTO.Request.Application.UserCreateAccount userCreateAccount)
        {
            var template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.UserCreateAccountEmail);
            if (template != null)
            {
                template.Body = template.Body.Replace("<email_address>", userCreateAccount.EmailAddress);
                template.Body = template.Body.Replace("<job_role>", userCreateAccount.JobRole);
                template.Body = template.Body.Replace("<organisation_name>", userCreateAccount.OrganisationName);
                template.Body = template.Body.Replace("<access_reason>", userCreateAccount.Reason);
                SendEmail(userCreateAccount.EmailAddress, template, true);
            }
        }

        private void SendEmail(string recipient, EmailTemplate emailTemplate, bool sendToSender = false)
        {
            if (string.IsNullOrEmpty(recipient)) throw new ArgumentNullException(nameof(recipient));
            if (emailTemplate == null) throw new ArgumentNullException(nameof(emailTemplate));
            var sender = _configuration.GetSection("Email:sender_address").GetConfigurationString(null, true);
            var displayName = _configuration.GetSection("General:product_name").GetConfigurationString(sender);
            try
            {
                var body = PopulateDynamicFields(emailTemplate.Body);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(sender, displayName),
                    IsBodyHtml = false,
                    Subject = emailTemplate.Subject,
                    Body = body,
                    To = { recipient }                    
                };
                if (sendToSender) mailMessage.To.Add(sender);
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

        private string PopulateDynamicFields(string bodyText)
        {
            bodyText = bodyText.Replace("<address>", _configuration.GetSection("General:get_access_email_address").GetConfigurationString(string.Empty));
            bodyText = bodyText.Replace("<url>", _contextAccessor.HttpContext.GetBaseSiteUrl());            
            return bodyText;
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

        public List<EmailTemplate> GetEmailTemplates()
        {
            var functionName = "application.get_email_templates";
            var result = _dataService.ExecuteFunction<EmailTemplate>(functionName);
            return result;
        }
    }
}
