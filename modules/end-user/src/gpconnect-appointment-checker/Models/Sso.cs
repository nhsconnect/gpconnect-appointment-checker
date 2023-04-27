using Newtonsoft.Json;
namespace GpConnect.AppointmentChecker.Models;

public class Sso
{
    [JsonProperty("clientId")]
    public string ClientId { get; set; }

    [JsonProperty("clientSecret")]
    public string ClientSecret { get; set; }

    [JsonProperty("callbackPath")]
    public string CallbackPath { get; set; }

    [JsonProperty("authScheme")]
    public string AuthScheme { get; set; }

    [JsonProperty("authEndpoint")]
    public string AuthEndpoint { get; set; }
    
    [JsonProperty("tokenEndpoint")]
    public string TokenEndpoint { get; set; }

    [JsonProperty("challengeScheme")]
    public string ChallengeScheme { get; set; }

    [JsonProperty("metadataEndpoint")]
    public string MetadataEndpoint { get; set; }

    [JsonProperty("signedOutCallbackPath")]
    public string SignedOutCallbackPath { get; set; }
}
