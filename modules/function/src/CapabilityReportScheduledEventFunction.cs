using Amazon.Lambda.Core;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Net;
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
    private readonly StorageConfiguration _storageConfiguration;
    private ILambdaContext _lambdaContext;
    private List<string> _additionalOdsCodes = new List<string>();

    public CapabilityReportScheduledEventFunction()
    {
        _secretManager = new SecretManager();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new UriBuilder(apiUrl).Uri;
        _httpClient.Timeout = TimeSpan.FromMinutes(15);

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<HttpStatusCode> FunctionHandler(FunctionRequest input, ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;
        _lambdaContext.Logger.LogLine("Firing up CapabilityReportScheduledEventFunction");
        await Reset(Objects.Transient);
        _additionalOdsCodes = input.OdsCodes;
        var messages = await AddMessagesToQueue();
        return await GenerateMessages(messages);
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

    private async Task<HttpStatusCode> GenerateMessages(List<MessagingRequest> messagingRequests)
    {
        for (var i = 0; i < messagingRequests.Count; i++)
        {
            var json = new StringContent(JsonConvert.SerializeObject(messagingRequests[i], null, _options),
                Encoding.UTF8,
                MediaTypeHeaderValue.Parse("application/json").MediaType);

            var response = await _httpClient.PostWithHeadersAsync("/reporting/createinteractionmessage", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);
            response.EnsureSuccessStatusCode();
        }
        _lambdaContext.Logger.LogLine($"Completed generation of {messagingRequests.Count} messages");
        await AddCompletionMessage();
        return HttpStatusCode.OK;
    }

    private async Task AddCompletionMessage()
    {
        _lambdaContext.Logger.LogLine($"Adding completion message");

        var json = new StringContent("{ \"ReportingStatus\" : [ \"OK\" ] }", Encoding.UTF8, MediaTypeHeaderValue.Parse("text/plain").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/messaging", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        _lambdaContext.Logger.LogLine("Response from AddCompletionMessage");
        _lambdaContext.Logger.LogLine(await response.Content.ReadAsStringAsync());
        response.EnsureSuccessStatusCode();

        _lambdaContext.Logger.LogLine($"Finished adding completion message");

    }

    private async Task<List<string>?> LoadSource()
    {
        var sourceOdsCodes = await StorageManager.Get<List<string>>(new StorageDownloadRequest()
        {
            BucketName = _storageConfiguration.BucketName,
            Key = _storageConfiguration.SourceObject
        });
        return sourceOdsCodes;
    }

    private async Task Reset(string objectPrefix)
    {
        _lambdaContext.Logger.LogLine("Purging S3 bucket");

        await StorageManager.Purge(new StorageListRequest
        {
            BucketName = _storageConfiguration.BucketName,
            ObjectPrefix = objectPrefix
        });
    }

    private async Task<List<MessagingRequest>> AddMessagesToQueue()
    {
        var sourceOdsCodes = await LoadSource();
        var messages = new List<MessagingRequest>();
        if (sourceOdsCodes != null && sourceOdsCodes.Count > 0)
        {            
            sourceOdsCodes.AddRange(_additionalOdsCodes);
            var capabilityReports = await GetCapabilityReports();

            var batchSize = 20;
            var iterationCount = sourceOdsCodes.Count / batchSize;
            var x = 0;
            var y = 0;
            var messageGroupId = Guid.NewGuid();

            for (var i = 0; i < capabilityReports.Count; i++)
            {
                while (y <= iterationCount)
                {
                    messages.Add(new MessagingRequest()
                    {
                        OdsCodes = sourceOdsCodes.GetRange(x, x + batchSize > sourceOdsCodes.Count ? sourceOdsCodes.Count - x : batchSize),
                        ReportName = capabilityReports[i].ReportName,
                        InteractionId = capabilityReports[i].InteractionId,
                        MessageGroupId = messageGroupId
                    });
                    x += batchSize;
                    y++;
                }
            }
        }
        _lambdaContext.Logger.LogLine("Adding " + messages.Count + " to queue");
        return messages;
    }
}
