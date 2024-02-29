using Amazon.Lambda.Core;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Globalization;
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

    public async Task<HttpStatusCode> FunctionHandler(ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;
        await Reset(Objects.Interaction, Objects.Transient);
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
        return HttpStatusCode.OK;
    }

    private async Task<List<ReportSource>> LoadReportSource()
    {
        var reportSource = await StorageManager.Get(new StorageDownloadRequest()
        {
            BucketName = _storageConfiguration.BucketName,
            Key = _storageConfiguration.SourceObject
        });

        using var reader = new StringReader(reportSource);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        var records = csvReader.GetRecords<ReportSource>().ToList();
        return records;
    }

    private async Task Reset(params string[] objectPrefix)
    {
        foreach (var key in objectPrefix)
        {
            await StorageManager.Purge(new StorageListRequest
            {
                BucketName = _storageConfiguration.BucketName,
                ObjectPrefix = key
            });
        }
    }

    private async Task<List<MessagingRequest>> AddMessagesToQueue()
    {
        var reportSource = await LoadReportSource();
        var messages = new List<MessagingRequest>();

        if (reportSource != null && reportSource.Count > 0)
        {
            var capabilityReports = await GetCapabilityReports();

            var batchSize = 20;
            var iterationCount = reportSource.Count / batchSize;
            var x = 0;
            var y = 0;
            var messageGroupId = Guid.NewGuid();            

            for (var i = 0; i < capabilityReports.Count; i++)
            {
                var interactionRequest = new InteractionRequest { InteractionId = capabilityReports[i].InteractionId, ReportName = capabilityReports[i].ReportName };
                var interactionBytes = JsonConvert.SerializeObject(interactionRequest, _options);

                await StorageManager.Post(new StorageUploadRequest
                {
                    BucketName = _storageConfiguration.BucketName,
                    Key = capabilityReports[i].InteractionKey,
                    InputBytes = Encoding.UTF8.GetBytes(interactionBytes)
                });

                while (y <= iterationCount)
                {
                    messages.Add(new MessagingRequest()
                    {
                        ReportSource = reportSource.GetRange(x, x + batchSize > reportSource.Count ? reportSource.Count - x : batchSize),
                        ReportName = capabilityReports[i].ReportName,
                        InteractionId = capabilityReports[i].InteractionId,
                        MessageGroupId = messageGroupId
                    });
                    x += batchSize;
                    y++;
                }
            }
        }
        return messages;
    }
}
