using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using NotificationConfiguration = GpConnect.AppointmentChecker.Function.Configuration.NotificationConfiguration;

namespace GpConnect.AppointmentChecker.Function;

public class SQSEventFunction
{
    private readonly HttpClient _httpClient;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private readonly NotificationConfiguration _notificationConfiguration;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private ILambdaContext _lambdaContext;
    private List<string> _distributionList = new List<string>();

    public SQSEventFunction()
    {
        _secretManager = new SecretManager();
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _notificationConfiguration = JsonConvert.DeserializeObject<NotificationConfiguration>(_secretManager.Get("notification-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new UriBuilder(apiUrl).Uri;

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;
        _lambdaContext.Logger.LogInformation("Firing SQSEventFunction");

        var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();
        foreach (var message in evnt.Records)
        {
            try
            {
                var reportInteraction = await ProcessMessageAsync(message);                
                var response = await GenerateCapabilityReport(reportInteraction);
                if (response.IsSuccessStatusCode)
                {
                    await GenerateTransientJsonForReport(reportInteraction.InteractionKeyJson, response);
                }
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = message.MessageId });
            }
        }
        return new SQSBatchResponse(batchItemFailures);
    }

    private async Task<ReportInteraction?> ProcessMessageAsync(SQSEvent.SQSMessage message)
    {
        try
        {
            ReportInteraction? reportInteraction = null;
            var messageRequest = JsonConvert.DeserializeObject<MessagingRequest>(message.Body);
            if (messageRequest != null)
            {
                reportInteraction = new ReportInteraction()
                {
                    OdsCodes = messageRequest.OdsCodes,
                    ReportName = messageRequest.ReportName,
                    InteractionId = messageRequest.InteractionId
                };                
            }
            return reportInteraction;
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.StackTrace);
            throw;
        }
    }

    private async Task<HttpResponseMessage?> GenerateCapabilityReport(ReportInteraction reportInteraction)
    {
        try
        {
            var json = new StringContent(JsonConvert.SerializeObject(reportInteraction, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

            var response = await _httpClient.PostWithHeadersAsync("/reporting/createinteractiondata", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);

            _lambdaContext.Logger.LogLine($"StatusCode from GenerateCapabilityReport: {response.StatusCode}");
            return response;
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.Message);
            throw;
        }
    }

    private async Task<string?> GenerateTransientJsonForReport(string interactionKeyJson, HttpResponseMessage? httpResponseMessage)
    {
        if (httpResponseMessage != null)
        {
            var inputBytes = await GetByteArray(httpResponseMessage);
            var url = await StorageManager.Post(new StorageUploadRequest()
            {
                BucketName = _storageConfiguration.BucketName,
                Key = interactionKeyJson,
                InputBytes = inputBytes
            });
            return url;
        }
        return null;
    }

    private async Task<string?> PostCapabilityReport(string interactionKey, HttpResponseMessage? response)
    {
        if (response != null)
        {
            var inputBytes = await GetByteArray(response);
            var url = await StorageManager.Post(new StorageUploadRequest()
            {
                BucketName = _storageConfiguration.BucketName,
                Key = interactionKey,
                InputBytes = inputBytes
            });
            return url;
        }
        return null;
    }

    private async Task EmailCapabilityReport(ReportInteraction reportInteraction)
    {
        var notification = new MessagingNotificationFunctionRequest()
        {
            ApiKey = _notificationConfiguration.CapabilityReportingApiKey,
            EmailAddresses = _distributionList,
            TemplateId = _notificationConfiguration.CapabilityReportingTemplateId,
            TemplateParameters = new Dictionary<string, dynamic> {
                { "report_name", reportInteraction.ReportName },
                { "interaction_id", reportInteraction.InteractionId },
                { "pre_signed_url", reportInteraction.PreSignedUrl },
                { "date_generated", DateTime.Now.ToString("dd MMMM yyyy HH:mm:ss") }
            }
        };

        var json = new StringContent(JsonConvert.SerializeObject(notification, null, _options),
           Encoding.UTF8,
           MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/notification", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        response.EnsureSuccessStatusCode();
    }

    private async Task<byte[]> GetByteArray(HttpResponseMessage response)
    {
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsByteArrayAsync();
    }

}
