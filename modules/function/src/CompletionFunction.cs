using Amazon.Lambda.Core;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Diagnostics;
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
    private string _reportName;
    private Stopwatch _stopwatch;

    public CompletionFunction()
    {
        _secretManager = new SecretManager();
        _stopwatch = new Stopwatch();
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
    public async Task<HttpStatusCode> FunctionHandler(CompletionFunctionRequest completionFunctionRequest, ILambdaContext lambdaContext)
    {
        _stopwatch.Start();
        _lambdaContext = lambdaContext;
        _reportName = completionFunctionRequest.ReportName;
        _distributionList = completionFunctionRequest.DistributionList;
        await BundleUpJsonResponsesAndSendReport();
        _stopwatch.Stop();
        _lambdaContext.Logger.LogInformation($"CompletionFunction took {_stopwatch.Elapsed.TotalSeconds} to process.");
        return HttpStatusCode.OK;
    }

    private async Task BundleUpJsonResponsesAndSendReport()
    {
        var bucketObjects = await StorageManager.GetObjects(new StorageListRequest
        {
            BucketName = _storageConfiguration.BucketName,
            ObjectPrefix = Objects.Transient
        });

        _lambdaContext.Logger.LogLine($"Bundling up JSON from the objects {string.Join(", ", bucketObjects.Select(x => x.Key).ToArray())}");

        var stringBuilder = new StringBuilder();
        foreach (var item in bucketObjects)
        {
            var bucketObject = await StorageManager.Get(new StorageDownloadRequest { BucketName = item.BucketName, Key = item.Key });
            stringBuilder.Append(bucketObject + ",");
        }
        await CreateReport($"[{stringBuilder}]");
    }

    private async Task CreateReport(string jsonData)
    {
        var interactionDetails = await StorageManager.GetObjects(new StorageListRequest
        {
            BucketName = _storageConfiguration.BucketName,
            ObjectPrefix = Objects.Interaction
        });

        var reportCreationRequest = new ReportCreationRequest
        {
            JsonData = jsonData
        };

        if (interactionDetails != null && interactionDetails.Count > 0)
        {
            var interactionObject = await StorageManager.Get<ReportInteraction>(new StorageDownloadRequest { BucketName = interactionDetails[0].BucketName, Key = interactionDetails[0].Key });
            reportCreationRequest.ReportName = interactionObject.ReportName;
        }
        else
        {
            reportCreationRequest.ReportName = _reportName;
        }

        _lambdaContext.Logger.LogLine($"Generating report with the key {reportCreationRequest.ReportKey}");

        var json = new StringContent(JsonConvert.SerializeObject(reportCreationRequest, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/reporting/createinteractionreport", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);        
        response.EnsureSuccessStatusCode();

        var fileStream = await response.Content.ReadAsStreamAsync();
        var byteArray = StreamExtensions.UseBufferedStream(fileStream);

        reportCreationRequest.ReportBytes = byteArray;
        await PostReport(reportCreationRequest);
    }

    private async Task PostReport(ReportCreationRequest reportCreationRequest)
    {
        var getUrl = await StorageManager.Post(new StorageUploadRequest
        {
            BucketName = _storageConfiguration.BucketName,
            InputBytes = reportCreationRequest.ReportBytes,
            Key = reportCreationRequest.ReportKey
        });
        await EmailCapabilityReport(reportCreationRequest, getUrl);
    }

    private async Task EmailCapabilityReport(ReportCreationRequest reportCreationRequest, string preSignedUrl)
    {
        var notification = new MessagingNotificationFunctionRequest()
        {
            ApiKey = _notificationConfiguration.CapabilityReportingApiKey,
            EmailAddresses = _distributionList,
            TemplateId = _notificationConfiguration.CapabilityReportingTemplateId,
            TemplateParameters = new Dictionary<string, dynamic> {
                { "report_name", reportCreationRequest.ReportName },
                { "pre_signed_url", preSignedUrl },
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
