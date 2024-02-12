using Amazon.Lambda.Core;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
namespace GpConnect.AppointmentChecker.Function;

public class CapabilityReportScheduledEventFunction
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly NotificationConfiguration _notificationConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private List<string> _distributionList = new List<string>();

    public CapabilityReportScheduledEventFunction()
    {
        _httpClient = new HttpClient();
        _secretManager = new SecretManager();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _notificationConfiguration = JsonConvert.DeserializeObject<NotificationConfiguration>(_secretManager.Get("notification-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");
        _httpClient.BaseAddress = new UriBuilder(apiUrl).Uri;

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task FunctionHandler(FunctionRequest input, ILambdaContext context)
    {
        _distributionList = input.DistributionList;
        await GetCapabilityReport(input, context);
    }

    private async Task GetCapabilityReport(FunctionRequest input, ILambdaContext context)
    {
        var capabilityReports = await GetCapabilityReports();
        for (var i = 0; i < capabilityReports.Count; i++)
        {
            await GenerateCapabilityReport(new ReportInteraction()
            {
                OdsCodes = input.OdsCodes,
                ReportName = capabilityReports[i].ReportName,
                InteractionId = capabilityReports[i].InteractionId
            });
        }
    }

    private async Task GenerateCapabilityReport(ReportInteraction reportInteraction)
    {
        var json = new StringContent(JsonConvert.SerializeObject(reportInteraction, null, _options),
           Encoding.UTF8,
           MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/reporting/interaction", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        response.EnsureSuccessStatusCode();
        var result = await GetByteArray(response);
        if (result != null)
        {
            await PostCapabilityReport(reportInteraction, result);
            //await EmailCapabilityReport(reportInteraction, result);
        }
    }

    private async Task PostCapabilityReport(ReportInteraction reportInteraction, byte[] result)
    {
        StorageManager.Post(new StorageUploadRequest()
        {
            BucketName = _storageConfiguration.BucketName,
            Key = reportInteraction.InteractionKey,
            InputBytes = result
        });
    }

    private async Task EmailCapabilityReport(ReportInteraction reportInteraction, byte[] documentContent)
    {
        var notification = new MessagingNotificationFunctionRequest()
        {
            ApiKey = _notificationConfiguration.CapabilityReportingApiKey,
            EmailAddresses = _distributionList,
            TemplateId = _notificationConfiguration.CapabilityReportingTemplateId,
            FileUpload = new Dictionary<string, byte[]> { { "link_to_file", documentContent } },
            TemplateParameters = new Dictionary<string, dynamic> {
                { "report_name", reportInteraction.ReportName },
                { "interaction_id", reportInteraction.InteractionId },
                { "date_generated", DateTime.Now.ToString("F") }
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

    public async Task<List<CapabilityReport>> GetCapabilityReports()
    {
        var response = await _httpClient.GetWithHeadersAsync("/reporting/capabilitylist", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<CapabilityReport>>(body, _options);
    }
}
