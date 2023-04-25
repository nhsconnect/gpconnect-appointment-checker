namespace GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

public class Sso
{
    public string ClientId { get; set; }
    public string ClientSecret { get; set; }
    public string CallbackPath { get; set; }
    public string AuthScheme { get; set; }
    public string AuthEndpoint { get; set; }
    public string TokenEndpoint { get; set; }
    public string ChallengeScheme { get; set; }
    public string MetadataEndpoint { get; set; }
    public string SignedOutCallbackPath { get; set; }
}
