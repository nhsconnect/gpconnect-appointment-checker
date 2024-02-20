using Amazon.Lambda.Core;
using Amazon.Lambda.SQSEvents;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Request;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
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

    public SQSEventFunction()
    {
        _secretManager = new SecretManager();
        _storageConfiguration = JsonConvert.DeserializeObject<StorageConfiguration>(_secretManager.Get("storage-configuration"));
        _endUserConfiguration = JsonConvert.DeserializeObject<EndUserConfiguration>(_secretManager.Get("enduser-configuration"));

        var apiUrl = _endUserConfiguration?.ApiBaseUrl ?? throw new ArgumentNullException("ApiBaseUrl");

        _httpClient = new HttpClient();
        _httpClient.BaseAddress = new UriBuilder(apiUrl).Uri;

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    [LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]
    public async Task<SQSBatchResponse> FunctionHandler(SQSEvent evnt, ILambdaContext lambdaContext)
    {
        _lambdaContext = lambdaContext;

        _lambdaContext.Logger.LogInformation("Firing SQSEventFunction");

        var batchItemFailures = new List<SQSBatchResponse.BatchItemFailure>();
        foreach (var message in evnt.Records)
        {
            _lambdaContext.Logger.LogInformation("Processing message: " + message.Body);
            try
            {
                await ProcessMessageAsync(message);
            }
            catch (Exception)
            {
                batchItemFailures.Add(new SQSBatchResponse.BatchItemFailure { ItemIdentifier = message.MessageId });
            }
        }
        return new SQSBatchResponse(batchItemFailures);
    }

    private async Task ProcessMessageAsync(SQSEvent.SQSMessage message)
    {
        try
        {
            var messageRequest = JsonConvert.DeserializeObject<MessagingRequest>(message.Body);
            if (messageRequest != null)
            {
                await GenerateCapabilityReport(new ReportInteraction()
                {
                    OdsCodes = messageRequest.OdsCodes,
                    ReportName = messageRequest.ReportName,
                    InteractionId = messageRequest.InteractionId
                });
            }
            await Task.CompletedTask;

        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.StackTrace);
            throw;
        }
    }

    private async Task GenerateCapabilityReport(ReportInteraction reportInteraction)
    {
        try
        {
            var json = new StringContent(JsonConvert.SerializeObject(reportInteraction, null, _options),
               Encoding.UTF8,
               MediaTypeHeaderValue.Parse("application/json").MediaType);

            var response = await _httpClient.PostWithHeadersAsync("/reporting/createinteractiondata", new Dictionary<string, string>()
            {
                [Headers.UserId] = _endUserConfiguration.UserId,
                [Headers.ApiKey] = _endUserConfiguration.ApiKey
            }, json);

            if (response.IsSuccessStatusCode)
            {
                await GenerateTransientJsonForReport(reportInteraction.InteractionKeyJson, response);
            }
        }
        catch (Exception e)
        {
            _lambdaContext.Logger.LogError(e.StackTrace);
            throw;
        }
    }

        private async Task<string?> GenerateTransientJsonForReport(string interactionKeyJson, HttpResponseMessage? response)
        {
            if (response != null)
            {
                var inputBytes = await GetByteArray(response);
                var url = await StorageManager.Post(new StorageUploadRequest()
                {
                    BucketName = _storageConfiguration.BucketName,
                    Key = interactionKeyJson,
                    InputBytes = inputBytes
                });
                return url;
            }
            return null;
        }

        private async Task<byte[]> GetByteArray(HttpResponseMessage response)
        {
            return await response.Content.ReadAsByteArrayAsync();
        }

    }
