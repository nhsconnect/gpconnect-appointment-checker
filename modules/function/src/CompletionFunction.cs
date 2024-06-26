using Amazon.Lambda.Core;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
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
    private DistributionListRequest _distributionList = new DistributionListRequest();
    private List<ReportFilterRequest> _reportFilterRequest = new List<ReportFilterRequest>();
    private Stopwatch _stopwatch;

    public CompletionFunction()
    {
        _secretManager = new SecretManager();
        _stopwatch = new Stopwatch();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _notificationConfiguration = JsonConvert.DeserializeObject<NotificationConfiguration>(_secretManager.Get("notification-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient
        {
            BaseAddress = new UriBuilder(apiUrl).Uri
        };

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task<HttpStatusCode> FunctionHandler(ILambdaContext lambdaContext)
    {
        _stopwatch.Start();
        _lambdaContext = lambdaContext;
        var completionFunctionRequest = await StorageManager.Get<CompletionFunctionRequest>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = $"{Objects.Distribution}.json" });
        if (completionFunctionRequest != null)
        {
            _reportFilterRequest = completionFunctionRequest.ReportFilter;
            _distributionList = completionFunctionRequest.DistributionList;
            await CreateReport();
            _stopwatch.Stop();
            _lambdaContext.Logger.LogInformation($"CompletionFunction took {_stopwatch.Elapsed:%m} minutes {_stopwatch.Elapsed:%s} seconds to process");
            return HttpStatusCode.OK;
        }
        return HttpStatusCode.BadRequest;
    }

    private async Task CreateReport()
    {
        var keyObjects = await StorageManager.GetObjects(new StorageListRequest
        {
            BucketName = _storageConfiguration.BucketName,
            ObjectPrefix = $"{Objects.Key}"
        });

        foreach (var keyObject in keyObjects)
        {
            var key = await StorageManager.Get<InteractionKey>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = keyObject.Key });

            if (key != null)           
            {
                var reportCreationRequest = new ReportCreationRequest
                {
                    ReportFilter = _reportFilterRequest,
                    ReportName = key.ReportName,
                    ReportId = key.ReportId
                };

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
        }        
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
        var selectedEmailAddresses = _distributionList.SelectedRecipients?.Where(x => x.ReportId.ToUpper() == reportCreationRequest.ReportId.ToUpper()).SelectMany(x => x.ReportRecipients);
        if (selectedEmailAddresses != null)
        {
            _distributionList.AllRecipients.AddRange(selectedEmailAddresses);
        }

        var notification = new MessagingNotificationFunctionRequest()
        {
            ApiKey = _notificationConfiguration.CapabilityReportingApiKey,
            EmailAddresses = _distributionList.AllRecipients.DistinctBy(x => x).ToList(),
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
