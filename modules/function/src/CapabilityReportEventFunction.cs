using Amazon.Lambda.Core;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
using System.Collections.Concurrent;
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
        await Reset(Objects.Transient, Objects.Key, Objects.Completion);
        var rolesSource = await StorageManager.Get<List<string>>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = _storageConfiguration.RolesObject });
        var odsList = await GetOdsData(rolesSource.ToArray());
        var messages = await AddMessagesToQueue(odsList);
        var statusCode = await GenerateMessages(messages);
        return statusCode;
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

    private async Task<IEnumerable<MessagingRequest>> AddMessagesToQueue(string[] odsList)
    {
        var codesSuppliers = await LoadDataSource();
        var dataSource = codesSuppliers.Where(x => odsList.Contains(x.OdsCode)).ToList();
        var messages = new ConcurrentDictionary<MessagingRequest, int>();

        if (dataSource != null && dataSource.Any())
        {
            var dataSourceCount = dataSource.Count;
            var capabilityReports = await GetCapabilityReports();

            await Parallel.ForEachAsync(capabilityReports, _parallelOptions, async (capabilityReport, ct) =>
            {
                var interactionRequest = new InteractionRequest
                {
                    WorkflowId = capabilityReport.Workflow?.FirstOrDefault(),
                    InteractionId = capabilityReport.Interaction?.FirstOrDefault(),
                    ReportName = capabilityReport.ReportName,
                    ReportId = capabilityReport.ReportId
                };

                var interactionBytes = JsonConvert.SerializeObject(interactionRequest, _options);

                await StorageManager.Post(new StorageUploadRequest
                {
                    BucketName = _storageConfiguration.BucketName,
                    Key = capabilityReport.ObjectKey,
                    InputBytes = Encoding.UTF8.GetBytes(interactionBytes)
                });

                var start = 0;
                var increment = 1000;

                while (start < dataSourceCount)
                {
                    var requests = dataSource.GetRange(start, !((start + increment) > dataSourceCount) ? increment : dataSourceCount - start);
                    if (requests.Any())
                    {
                        foreach (var request in requests)
                        {
                            messages.TryAdd(new MessagingRequest()
                            {
                                DataSource = new() { OdsCode = request.OdsCode, SupplierName = request.SupplierName },
                                ReportName = capabilityReport.ReportName,
                                Interaction = capabilityReport.Interaction,
                                Workflow = capabilityReport.Workflow,
                                MessageGroupId = capabilityReport.MessageGroupId,
                                ReportId = capabilityReport.ReportId
                            }, Environment.CurrentManagedThreadId);
                        }
                    }
                    start += increment;
                }
            });
        }
        return messages.Select(x => x.Key);
    }

    private async Task<HttpStatusCode> GenerateMessages(IEnumerable<MessagingRequest> messagingRequests)
    {
        await Parallel.ForEachAsync(messagingRequests, _parallelOptions, async (messagingRequest, ct) =>
        {
            var json = new StringContent(JsonConvert.SerializeObject(messagingRequest, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

            _lambdaContext.Logger.LogLine(await json.ReadAsStringAsync());

            await _httpClient.PostWithHeadersAsync("/reporting/createinteractionmessage", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);
        });
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
            var response = await StorageManager.Purge(new StorageListRequest
            {
                BucketName = _storageConfiguration.BucketName,
                ObjectPrefix = objectPrefix[i]
            }, _lambdaContext);

            if (response != null)
            {
                _lambdaContext.Logger.LogLine(response.DeletedObjects.Count.ToString());
                foreach (var deletedObject in response.DeletedObjects)
                {
                    _lambdaContext.Logger.LogLine(deletedObject.Key);
                }
            }
        }
    }
}
