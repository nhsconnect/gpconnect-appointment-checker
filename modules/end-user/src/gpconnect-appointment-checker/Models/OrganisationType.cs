using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class OrganisationType
{
    [JsonProperty("organisationTypeId")] 
    public int OrganisationTypeId { get; set; }
    [JsonProperty("organisationTypeCode")]
    public string OrganisationTypeCode { get; set; }
    [JsonProperty("organisationTypeDescription")]
    public string OrganisationTypeDescription { get; set; }
}
