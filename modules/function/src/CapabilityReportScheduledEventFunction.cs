using Amazon.Lambda.Core;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Diagnostics;
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
    private Stopwatch _stopwatch;

    public CapabilityReportScheduledEventFunction()
    {
        _secretManager = new SecretManager();
        _stopwatch = new Stopwatch();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient
        {
            BaseAddress = new UriBuilder(apiUrl).Uri,
            Timeout = TimeSpan.FromMinutes(15)
        };

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<HttpStatusCode> FunctionHandler(ScheduledFunctionRequest scheduledFunctionRequest, ILambdaContext lambdaContext)
    {
        _stopwatch.Start();
        _lambdaContext = lambdaContext;
        await Reset(Objects.Key, Objects.Transient, Objects.Hierarchy);
        var odsList = await GetOdsData(scheduledFunctionRequest.OdsRoles);
        var messages = await AddMessagesToQueue(odsList);
        var list = await GenerateMessages(messages);
        _stopwatch.Stop();
        _lambdaContext.Logger.LogInformation($"CapabilityReportScheduledEventFunction took {_stopwatch.Elapsed:%m} minutes {_stopwatch.Elapsed:%s} seconds to process");
        return list;
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

    public async Task<string[]> GetOdsData(string[] roles)
    {
        var queryBuilder = new QueryBuilder
        {
            { "roles", string.Join(",", roles) }
        };

        var response = await _httpClient.GetWithHeadersAsync($"/organisation/ods{queryBuilder.ToQueryString()}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<string[]>(body, _options);
    }

    private async Task<HttpStatusCode> GenerateMessages(List<MessagingRequest> messagingRequests)
    {
        for (var i = 0; i < messagingRequests.Count; i++)
        {
            var json = new StringContent(JsonConvert.SerializeObject(messagingRequests[i], null, _options),
                Encoding.UTF8,
                MediaTypeHeaderValue.Parse("application/json").MediaType);

            _lambdaContext.Logger.LogLine(await json.ReadAsStringAsync());

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

    private async Task<List<DataSource>> LoadDataSource()
    {
        var reportSource = await StorageManager.Get(new StorageDownloadRequest()
        {
            BucketName = _storageConfiguration.BucketName,
            Key = _storageConfiguration.SourceObject
        });

        using var reader = new StringReader(reportSource);
        using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);
        return csvReader.GetRecords<DataSource>().DistinctBy(x => x.OdsCode).OrderBy(x => x.OdsCode).ToList();
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

    private async Task<List<MessagingRequest>> AddMessagesToQueue(string[] odsList)
    {
        var codesSuppliers = await LoadDataSource();
        var dataSource = codesSuppliers.Where(x => odsList.Contains(x.OdsCode)).ToList();
        var hierarchyKey = await PersistOrganisationHierarchy(dataSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList());

        var messages = new List<MessagingRequest>();

        if (dataSource != null && dataSource.Any())
        {
            var dataSourceCount = dataSource.Count;
            var capabilityReports = await GetCapabilityReports();

            var batchSize = 20;
            var iterationCount = dataSourceCount / batchSize;

            for (var i = 0; i < capabilityReports.Count; i++)
            {
                var x = 0;
                var y = 0;

                var messageGroupId = Guid.NewGuid();

                var interactionRequest = new InteractionRequest
                {
                    WorkflowId = capabilityReports[i].Workflow != null ? capabilityReports[i].Workflow.FirstOrDefault() : null,
                    InteractionId = capabilityReports[i].Interaction != null ? capabilityReports[i].Interaction.FirstOrDefault() : null,
                    ReportName = capabilityReports[i].ReportName,
                    ReportId = capabilityReports[i].ReportId
                };

                var interactionBytes = JsonConvert.SerializeObject(interactionRequest, _options);

                await StorageManager.Post(new StorageUploadRequest
                {
                    BucketName = _storageConfiguration.BucketName,
                    Key = capabilityReports[i].ObjectKey,
                    InputBytes = Encoding.UTF8.GetBytes(interactionBytes)
                });

                while (y <= iterationCount)
                {
                    messages.Add(new MessagingRequest()
                    {
                        DataSource = dataSource.GetRange(x, x + batchSize > dataSourceCount ? dataSourceCount - x : batchSize),
                        ReportName = capabilityReports[i].ReportName,
                        Interaction = capabilityReports[i].Interaction,
                        Workflow = capabilityReports[i].Workflow,
                        MessageGroupId = messageGroupId,
                        ReportId = capabilityReports[i].ReportId,
                        HierarchyKey = hierarchyKey
                    });
                    x += batchSize;
                    y++;
                }
            }
        }
        return messages;
    }

    private async Task<string> PersistOrganisationHierarchy(List<string> odsCodes)
    {
        var json = new StringContent(JsonConvert.SerializeObject(odsCodes, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostWithHeadersAsync("/organisation/hierarchy", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        response.EnsureSuccessStatusCode();
        var fileStream = await response.Content.ReadAsStreamAsync();
        var byteArray = StreamExtensions.UseBufferedStream(fileStream);
        var hierarchyKey = $"{Objects.Hierarchy}_{DateTime.UtcNow.Ticks}".ToLower();

        await StorageManager.Post(new StorageUploadRequest
        {
            BucketName = _storageConfiguration.BucketName,
            InputBytes = byteArray,
            Key = $"{hierarchyKey}.json"
        });

        return hierarchyKey;
    }
}
