using Amazon.Lambda.Core;
using GpConnect.AppointmentChecker.Function.Configuration;
using GpConnect.AppointmentChecker.Function.DTO.Response.Message;
using GpConnect.AppointmentChecker.Function.Helpers;
using GpConnect.AppointmentChecker.Function.Helpers.Constants;
using Newtonsoft.Json;
using System.Net;

namespace GpConnect.AppointmentChecker.Function;

public class CompletionFunction
{
    private readonly HttpClient _httpClient;
    private readonly EndUserConfiguration _endUserConfiguration;
    private readonly JsonSerializerSettings _options;
    private readonly SecretManager _secretManager;
    private ILambdaContext _lambdaContext;

    public CompletionFunction()
    {
        _secretManager = new SecretManager();
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
    public async Task<HttpStatusCode> FunctionHandler(ILambdaContext lambdaContext)
    {
        Thread.Sleep(TimeSpan.FromSeconds(10));
        _lambdaContext = lambdaContext;
        var messageStatus = await CheckForMessagesInFlight();
        var processedAllMessages = messageStatus.MessagesAvailable == 0 && messageStatus.MessagesInFlight == 0;

        if(processedAllMessages)
        {
            _lambdaContext.Logger.LogInformation("FINISHED!");
        }
        return HttpStatusCode.OK;
    }

    private async Task<MessageStatus> CheckForMessagesInFlight()
    {
        var response = await _httpClient.GetWithHeadersAsync("/messaging/getmessagestatus", new Dictionary<string, string>()
        {
            [Headers.UserId] = _endUserConfiguration.UserId,
            [Headers.ApiKey] = _endUserConfiguration.ApiKey
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        var messageStatus = JsonConvert.DeserializeObject<MessageStatus> (body, _options);
        
        return messageStatus;
    }
}
