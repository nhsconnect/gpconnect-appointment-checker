using Amazon.Lambda.Core;
using Amazon.SecretsManager.Model.Internal.MarshallTransformations;
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

namespace GpConnect.AppointmentChecker.Function;

public class CapabilityReportEventFunction
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private ILambdaContext _lambdaContext;
    private Stopwatch _stopwatch;
    private ParallelOptions _parallelOptions;

    public CapabilityReportEventFunction()
    {
        _secretManager = new SecretManager();
        _stopwatch = new Stopwatch();
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));

        _parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0))
        };

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

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task<HttpStatusCode> FunctionHandler(ILambdaContext lambdaContext)
    {
        _stopwatch.Start();
        _lambdaContext = lambdaContext;
        await PurgeObjects();
        var rolesSource = await StorageManager.Get<List<string>>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = _storageConfiguration.RolesObject });
        var odsList = await GetOdsData(rolesSource.ToArray());
        var messages = await AddMessagesToQueue(odsList);
        return messages;
    }

    private async Task PurgeObjects()
    {
        var keyObjects = await StorageManager.GetObjects(new StorageListRequest { BucketName = _storageConfiguration.BucketName, ObjectPrefix = Objects.Key });
        await Parallel.ForEachAsync(keyObjects, _parallelOptions, async (keyObject, ct) =>
        {
            await Reset($"{Objects.Transient}_{keyObject.Key.SearchAndReplace(new Dictionary<string, string>() { { ".json", string.Empty }, { "key_", string.Empty } })}");
        });
        await Reset(Objects.Key, Objects.Completion);
    }

    private async Task<string[]> GetOdsData(string[] roles)
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

    private async Task<HttpStatusCode> AddMessagesToQueue(string[] odsList)
    {
        var codesSuppliers = await LoadDataSource();
        var dataSource = codesSuppliers.Where(x => odsList.Contains(x.OdsCode)).ToList();        

        if (dataSource != null && dataSource.Any())
        {
            var messages = new List<MessagingRequest>();
            var dataSourceCount = dataSource.Count;
            var capabilityReports = await GetCapabilityReports();

            for (var i = 0; i < capabilityReports.Count; i++)
            {
                var interactionRequest = new InteractionRequest
                {
                    WorkflowId = capabilityReports[i].Workflow?.FirstOrDefault(),
                    InteractionId = capabilityReports[i].Interaction?.FirstOrDefault(),
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

                for (var j = 0; j < dataSourceCount; j++)
                {
                    messages.Add(new MessagingRequest()
                    {
                        DataSource = new() { OdsCode = dataSource[j].OdsCode, SupplierName = dataSource[j].SupplierName },
                        ReportName = capabilityReports[i].ReportName,
                        Interaction = capabilityReports[i].Interaction,
                        Workflow = capabilityReports[i].Workflow,
                        MessageGroupId = capabilityReports[i].MessageGroupId,
                        ReportId = capabilityReports[i].ReportId
                    });
                }
                await GenerateMessages(messages);
            }
        }
        return HttpStatusCode.OK;
    }

    private async Task<HttpStatusCode> GenerateMessages(List<MessagingRequest> messagingRequest)
    {
        var json = new StringContent(JsonConvert.SerializeObject(messagingRequest, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        await _httpClient.PostWithHeadersAsync("/reporting/createinteractionmessage", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        }, json);

        return HttpStatusCode.OK;
    }

    private async Task<List<CapabilityReport>> GetCapabilityReports()
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
        for (int i = 0; i < objectPrefix.Length; i++)
        {
            await StorageManager.Purge(new StorageListRequest
            {
                BucketName = _storageConfiguration.BucketName,
                ObjectPrefix = objectPrefix[i]
            });
        }
    }
}
