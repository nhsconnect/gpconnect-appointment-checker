using Amazon.Lambda.Core;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Microsoft.AspNetCore.Http.Extensions;
using Newtonsoft.Json;
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

    public CapabilityReportEventFunction()
    {
        _secretManager = new SecretManager();
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
        _lambdaContext = lambdaContext;
        var odsList = await GetOdsData(scheduledFunctionRequest.OdsRoles);
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

    private async Task<List<MessagingRequest>> AddMessagesToQueue(string[] odsList)
    {
        var codesSuppliers = await LoadDataSource();
        var dataSource = codesSuppliers.Where(x => odsList.Contains(x.OdsCode)).ToList();
        var hierarchyKey = await StorageManager.GetObjectKey(new StorageListRequest() { BucketName = _storageConfiguration.BucketName, ObjectPrefix = Objects.Hierarchy });

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
}
