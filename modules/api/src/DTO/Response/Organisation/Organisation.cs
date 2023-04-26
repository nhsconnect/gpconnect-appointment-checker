using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class Organisation
{
    [JsonProperty("id")]
    public string OrganisationId { get; set; }

    [JsonProperty("name")]
    public string OrganisationName { get; set; }

    [JsonProperty("address")]
    public OrganisationAddress PostalAddress { get; set; }

    [JsonProperty("type")]
    public Type Type { get; set; }

    [JsonProperty("issue")]
    public List<Issue>? Issue { get; set; }

    [JsonProperty("errorCode")]
    public int ErrorCode { get; set; }

    [JsonProperty("errorText")]
    public string ErrorText { get; set; }

    public bool HasErrored => ErrorCode > 0;
}