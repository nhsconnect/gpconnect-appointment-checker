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
    private readonly LambdaConfiguration _lambdaConfiguration;
    private List<string> _distributionList = new List<string>();

    public CapabilityReportScheduledEventFunction()
    {
        _httpClient = new HttpClient();

        _secretManager = new SecretManager();
        var endUserConfiguration = _secretManager.Get("gpcac/enduser-configuration");
        var lambdaConfiguration = _secretManager.Get("gpcac/lambda-configuration");

        if (endUserConfiguration != null && lambdaConfiguration != null)
        {
            _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(endUserConfiguration);
            _lambdaConfiguration = JsonConvert.DeserializeObject<LambdaConfiguration>(lambdaConfiguration);
        }
        else
        {
            _endUserConfiguration = new EndUserConfiguration()
            {
                ApiKey = Environment.GetEnvironmentVariable("EndUserConfiguration:ApiKey"),
                ApiBaseUrl = Environment.GetEnvironmentVariable("EndUserConfiguration:ApiBaseUrl"),
                UserId = Environment.GetEnvironmentVariable("EndUserConfiguration:UserId"),
            };

            _lambdaConfiguration = new LambdaConfiguration()
            {
                ApiKey = Environment.GetEnvironmentVariable("LambdaConfiguration:ApiKey"),
                TemplateId = Environment.GetEnvironmentVariable("LambdaConfiguration:TemplateId")
            };
        }

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
            await GenerateCapabilityReport(new ReportExport()
            {
                OdsCodes = input.OdsCodes,
                ReportName = capabilityReports[i].ReportName,
                InteractionId = capabilityReports[i].InteractionId
            });
        }
    }

    private async Task GenerateCapabilityReport(ReportExport reportExport)
    {
        var json = new StringContent(JsonConvert.SerializeObject(reportExport, null, _options),
           Encoding.UTF8,
           MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/reporting/export", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        response.EnsureSuccessStatusCode();
        var result = await GetByteArray(response);
        if (result != null)
        {
            await SendCapabilityReport(reportExport, result);
        }
    }

    private async Task SendCapabilityReport(ReportExport reportExport, byte[] documentContent)
    {
        var notification = new MessagingNotificationFunctionRequest() 
        { 
            ApiKey = _lambdaConfiguration.ApiKey,
            EmailAddresses = _distributionList,
            TemplateId = _lambdaConfiguration.TemplateId,
            FileUpload = new Dictionary<string, byte[]> { { "link_to_file", documentContent } },
            TemplateParameters = new Dictionary<string, dynamic> { 
                { "report_name", reportExport.ReportName },
                { "interaction_id", reportExport.InteractionId },
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
