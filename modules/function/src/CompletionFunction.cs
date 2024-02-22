using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using Amazon.S3.Model;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response.Message;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using NotificationConfiguration = GpConnect.AppointmentChecker.Function.Configuration.NotificationConfiguration;

namespace GpConnect.AppointmentChecker.Function;

public class CompletionFunction
{
    private readonly HttpClient _httpClient;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private readonly NotificationConfiguration _notificationConfiguration;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private ILambdaContext _lambdaContext;
    private List<string> _distributionList = new List<string>();

    public CompletionFunction()
    {
        _secretManager = new SecretManager();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _notificationConfiguration = JsonConvert.DeserializeObject<NotificationConfiguration>(_secretManager.Get("notification-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new UriBuilder(apiUrl).Uri;

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task<HttpStatusCode> FunctionHandler(FunctionRequest functionRequest, ILambdaContext lambdaContext)
    {
        _distributionList = functionRequest.DistributionList;
        _lambdaContext = lambdaContext;

        _lambdaContext.Logger.LogInformation("FINISHED!");

        //await BundleUpJsonResponsesAndSendReport();
        return HttpStatusCode.OK;
    }

    //private async Task BundleUpJsonResponsesAndSendReport()
    //{
    //    var bucketObjects = await StorageManager.GetObjects(new StorageListRequest
    //    {
    //        BucketName = _storageConfiguration.BucketName,
    //        ObjectPrefix = Objects.Transient
    //    });

    //    foreach (var item in bucketObjects)
    //    {
    //        var request = new GetObjectRequest() { 
    //            Key = item.Key, 
    //            BucketName = item.BucketName 
    //        };
    //        var response = S3AccessControlList;

    //        _lambdaContext.Logger.LogInformation(item.Key);



    //    }
    //}

    private async Task<MessageStatus> CheckForMessagesInFlight()
    {
        var response = await _httpClient.GetWithHeadersAsync("/messaging/getmessagestatus", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var messageStatus = JsonConvert.DeserializeObject<MessageStatus> (body, _options);        
        return messageStatus;
    }

    private async Task<string?> PostCapabilityReport(string interactionKey, HttpResponseMessage? response)
    {
        if (response != null)
        {
            var inputBytes = await StreamExtensions.GetByteArray(response);
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

}
