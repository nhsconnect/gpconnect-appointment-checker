using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using CsvHelper;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.DTO.Response;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Headers;
using System.Text;
using ThirdParty.BouncyCastle.Utilities.IO.Pem;

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
                var response = await RouteReportRequest(reportRequest);

                if (response.IsSuccessStatusCode)
                {
                    await GenerateTransientJsonForReport(reportRequest.ObjectKeyJson, response);
                }
                await Task.CompletedTask;
            }
            catch (Exception)
            {
                batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = message.MessageId });
            }
        }
        var batchResponse = new SQSBatchResponse(batchItemFailures);
        _stopwatch.Stop();
        _lambdaContext.Logger.LogInformation($"SQSEventFunction with {evnt.Records.Count} records took {_stopwatch.Elapsed:%m} minutes {_stopwatch.Elapsed:%s} seconds to process");
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
                var hierarchySource = await StorageManager.Get<List<OrganisationHierarchy>>(new StorageDownloadRequest { BucketName = _storageConfiguration.BucketName, Key = messageRequest.HierarchyKey });

                reportInteraction = new ReportInteraction()
                {
                    ReportSource = messageRequest.DataSource.Select(x => new ReportSource() { OdsCode = x.OdsCode, SupplierName = x.SupplierName, OrganisationHierarchy = hierarchySource.FirstOrDefault(y => y.OdsCode == x.OdsCode) }).ToList(),
                    ReportName = messageRequest.ReportName,
                    Interaction = messageRequest.Interaction,
                    Workflow = messageRequest.Workflow,
                    ReportId = messageRequest.ReportId
                };
            }

            _lambdaContext.Logger.LogLine($"Generating data for ODS Codes {string.Join(", ", messageRequest.DataSource.Select(x => x.OdsCode).ToArray())}");
            return reportInteraction;
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.StackTrace);
            throw;
        }
    }

    private async Task<HttpResponseMessage?> RouteReportRequest(ReportInteraction reportInteraction)
    {
        try
        {
            var json = new StringContent(JsonConvert.SerializeObject(reportInteraction, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

            var response = await _httpClient.PostWithHeadersAsync("/reporting/routereportrequest", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);

            return response;
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.Message);
            throw;
        }
    }

    private async Task<string?> GenerateTransientJsonForReport(string objectKeyJson, HttpResponseMessage? httpResponseMessage)
    {
        if (httpResponseMessage != null)
        {
            var inputBytes = await StreamExtensions.GetByteArray(httpResponseMessage);
            var url = await StorageManager.Post(new StorageUploadRequest()
            {
                BucketName = _storageConfiguration.BucketName,
                Key = objectKeyJson,
                InputBytes = inputBytes
            });
            return url;
        }
        return null;
    }
}
