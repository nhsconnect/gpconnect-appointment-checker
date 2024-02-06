using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using Notify.Client;
using Notify.Models.Responses;

namespace GpConnect.AppointmentChecker.Api.Service;

public class NotificationService : INotificationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IOptions<NotificationConfig> _config;

    public NotificationService(ILogger<NotificationService> logger, IOptions<NotificationConfig> config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<int> PostNotificationAsync(NotificationCreateRequest notificationCreateRequest)
    {
        try
        {
            var apiKey = notificationCreateRequest.ApiKey ?? _config.Value.ApiKey;
            if (!string.IsNullOrEmpty(apiKey) && !string.IsNullOrEmpty(notificationCreateRequest.TemplateId.Trim()))
            {
                var client = new NotificationClient(apiKey);
                var emailNotificationResponse = new List<EmailNotificationResponse>();

                foreach (var emailAddress in notificationCreateRequest.EmailAddresses.Where(x => !string.IsNullOrWhiteSpace(x)))
                {
                    var templateParameters = notificationCreateRequest.TemplateParameters?.ToDictionary(pair => pair.Key, pair => Convert.ToString(pair.Value) == null ? string.Empty : pair.Value.ToString());
                    templateParameters.Add("url", notificationCreateRequest.RequestUrl);

                    if(notificationCreateRequest.FileUpload != null)
                    {
                        templateParameters.Add(notificationCreateRequest.FileUpload.FirstOrDefault().Key, NotificationClient.PrepareUpload(notificationCreateRequest.FileUpload.FirstOrDefault().Value as byte[]));
                    }

                    emailNotificationResponse.Add(client.SendEmail(emailAddress, notificationCreateRequest.TemplateId, templateParameters));
                }
                return emailNotificationResponse.Count;
            }
            _logger.LogWarning("Unable to send email as required values are not present.");
            return 0;
        }
        catch (Notify.Exceptions.NotifyClientException exc)
        {
            _logger.LogError(exc, $"Failed to send notification. Template Id is {notificationCreateRequest.TemplateId}");
            throw;
        }
    }
}