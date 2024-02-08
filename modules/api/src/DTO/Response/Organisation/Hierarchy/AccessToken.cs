using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

internal class AccessToken
{
    [JsonProperty("access_token")]
    public string? Token { get; set; }
}
