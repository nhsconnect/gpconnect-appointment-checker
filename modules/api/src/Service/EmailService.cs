using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Audit;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace GpConnect.AppointmentChecker.Api.Service
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly IAuditService _auditService;
        private readonly IDataService _dataService;
        private readonly IOptionsMonitor<General> _generalOptionsDelegate;
        private readonly IOptionsMonitor<Email> _emailOptionsDelegate;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly SmtpClient _smtpClient;
        private readonly Lazy<List<EmailTemplate>> _emailTemplates;

        public EmailService(SmtpClient smtpClient, IOptionsMonitor<General> generalOptionsDelegate, IOptionsMonitor<Email> emailOptionsDelegate, ILogger<EmailService> logger, IAuditService auditService, IHttpContextAccessor contextAccessor, IDataService dataService)
        {
            _logger = logger;
            _auditService = auditService;
            _generalOptionsDelegate = generalOptionsDelegate;
            _emailOptionsDelegate = emailOptionsDelegate;
            _smtpClient = smtpClient;
            _contextAccessor = contextAccessor;
            _dataService = dataService;
            _emailTemplates = new Lazy<List<EmailTemplate>>();
        }

        public bool SendUserStatusEmail(int userId, int userAccountStatusId, string recipient, bool userStatusChanged = false)
        {
            if (userStatusChanged)
            {
                EmailTemplate template = null;
                switch (userAccountStatusId)
                {
                    case (int)UserAccountStatus.Authorised:
                        template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.AuthorisedConfirmationEmail);
                        break;
                    case (int)UserAccountStatus.Deauthorised:
                        template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.DeauthorisedConfirmationEmail);
                        break;
                }

                if (template != null)
                {
                    return SendEmail(userId, recipient, template);
                }
            }
            return false;
        }

        public bool SendUserCreateAccountEmail(User createdUser, DTO.Request.Application.UserCreateAccount userCreateAccount)
        {
            var template = _emailTemplates.Value.FirstOrDefault(x => x.MailTemplate == MailTemplate.UserCreateAccountEmail);
            if (template != null)
            {
                template.Body = template.Body.Replace("<email_address>", userCreateAccount.EmailAddress);
                template.Body = template.Body.Replace("<job_role>", userCreateAccount.JobRole);
                template.Body = template.Body.Replace("<organisation_name>", userCreateAccount.OrganisationName);
                template.Body = template.Body.Replace("<access_reason>", userCreateAccount.Reason);
                return SendEmail(createdUser.UserId, userCreateAccount.EmailAddress, template, true, false);
            }
            return false;
        }

        private bool SendEmail(int userId, string recipient, EmailTemplate emailTemplate, bool sendToSender = false, bool sendToRecipient = true)
        {
            if (string.IsNullOrEmpty(recipient)) throw new ArgumentNullException(nameof(recipient));
            if (emailTemplate == null) throw new ArgumentNullException(nameof(emailTemplate));
            var sender = StringExtensions.Coalesce(_emailOptionsDelegate.CurrentValue.SenderAddress, _emailOptionsDelegate.CurrentValue.UserName);
            var displayName = StringExtensions.Coalesce(_generalOptionsDelegate.CurrentValue.ProductName, sender);
            try
            {
                var body = PopulateDynamicFields(emailTemplate.Body);
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(sender, displayName),
                    IsBodyHtml = false,
                    Subject = emailTemplate.Subject,
                    Body = body
                };
                if (sendToSender) mailMessage.To.Add(sender);
                if (sendToRecipient) mailMessage.To.Add(recipient);
                
                _smtpClient.Send(mailMessage);
                SendToAudit(userId, recipient, body);
                return true;
            }
            catch (WebException webException)
            {
                _logger?.LogError(webException, "A connectivity error has occurred while attempting to send an email");
                return false;
            }
            catch (TimeoutException timeoutException)
            {
                _logger?.LogError(timeoutException, "A timeout error has occurred while attempting to send an email");
                return false;
            }
            catch (ArgumentNullException argumentNullException)
            {
                _logger?.LogError(argumentNullException, "One of the required arguments for sending an email is empty");
                return false;
            }
            catch (SmtpException smtpException)
            {
                _logger?.LogError(smtpException, "An SMTP error has occurred while attempting to send an email");
                return false;
            }
            catch (Exception exception)
            {
                _logger?.LogError(exception, "A general error has occurred while attempting to send an email");
                return false;
            }
        }

        private string PopulateDynamicFields(string bodyText)
        {
            bodyText = bodyText.Replace("<address>", _generalOptionsDelegate.CurrentValue.GetAccessEmailAddress);
            bodyText = bodyText.Replace("<url>", _contextAccessor.HttpContext.GetBaseSiteUrl());
            return bodyText;
        }

        private void SendToAudit(int userId, string recipient, string details)
        {
            var auditEntry = new Entry
            {    
                UserId = userId,
                Item1 = StringExtensions.Coalesce(_emailOptionsDelegate.CurrentValue.SenderAddress, _emailOptionsDelegate.CurrentValue.UserName),
                Item2 = recipient,
                Details = details,
                EntryTypeId = (int)AuditEntryType.EmailSent
            };
            _auditService.AddEntry(auditEntry);
        }

        public async Task<List<EmailTemplate>> GetEmailTemplates()
        {
            var functionName = "application.get_email_templates";
            var result = await _dataService.ExecuteQuery<EmailTemplate>(functionName);
            return result;
        }
    }
}
