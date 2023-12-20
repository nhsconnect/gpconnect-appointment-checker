using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;

public class OrganisationCoding
{
    [JsonProperty("display")]
    public string OrganisationTypeDisplay { get; set; }
}