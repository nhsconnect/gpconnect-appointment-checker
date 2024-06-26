using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace GpConnect.AppointmentChecker.Function;

public class SQSEventFunction
{
    private readonly HttpClient _httpClient;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly StorageConfiguration _storageConfiguration;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private ILambdaContext _lambdaContext;
    private Stopwatch _stopwatch;

    public SQSEventFunction()
    {
        _secretManager = new SecretManager();
        _stopwatch = new Stopwatch();
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));

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
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;
        _stopwatch.Start();
        var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();

        foreach (var message in evnt.Records)
        {
            try
            {
                var reportRequest = await ProcessMessageAsync(message);
                await RouteReportRequest(reportRequest);
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = message.MessageId });
            }
        }
        var batchResponse = new SQSBatchResponse(batchItemFailures);
        _stopwatch.Stop();
        return batchResponse;
    }

    private async Task<ReportInteraction?> ProcessMessageAsync(SQSEvent.SQSMessage message)
    {
        try
        {
            ReportInteraction? reportInteraction = null;
            var messageRequest = JsonConvert.DeserializeObject<MessagingRequest>(message.Body);
            if (messageRequest != null)
            {
                var json = new StringContent(JsonConvert.SerializeObject(messageRequest.DataSource.Select(x => x.OdsCode).ToList(), null, _options),
                    Encoding.UTF8,
                    MediaTypeHeaderValue.Parse("application/json").MediaType);

                var response = await _httpClient.PostWithHeadersAsync("/hierarchy", new Dictionary<string, string>()
                {
                    [Headers.UserId] = _endUserConfiguration.UserId,
                    [Headers.ApiKey] = _endUserConfiguration.ApiKey
                }, json);

                response.EnsureSuccessStatusCode();

                if (response != null)
                {
                    var body = await response.Content.ReadAsStringAsync();
                    var hierarchySource = JsonConvert.DeserializeObject<List<OrganisationHierarchy>>(body, _options);

                    reportInteraction = new ReportInteraction()
                    {
                        ReportSource = messageRequest.DataSource.Select(x => new ReportSource() { OdsCode = x.OdsCode, SupplierName = x.SupplierName, OrganisationHierarchy = hierarchySource.FirstOrDefault(y => y.OdsCode == x.OdsCode) }).ToList(),
                        ReportName = messageRequest.ReportName,
                        Interaction = messageRequest.Interaction,
                        Workflow = messageRequest.Workflow,
                        ReportId = messageRequest.ReportId
                    };
                }
            }
            return reportInteraction;
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.StackTrace);
            throw;
        }
    }

    private async Task RouteReportRequest(ReportInteraction reportInteraction)
    {
        try
        {
            var json = new StringContent(JsonConvert.SerializeObject(reportInteraction, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

            await _httpClient.PostWithHeadersAsync("/reporting/routereportrequest", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.Message);
            throw;
        }
    }
}
