using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class Organisation
{
    [JsonProperty("organisationId")] 
    public int OrganisationId { get; set; }
    [JsonProperty("odsCode")]
    public string ODSCode { get; set; }
    [JsonProperty("organisationTypeName")]
    public string OrganisationTypeName { get; set; }
    [JsonProperty("organisationName")]
    public string OrganisationName { get; set; }
    [JsonProperty("addressLine1")]
    public string AddressLine1 { get; set; }
    [JsonProperty("addressLine2")]
    public string AddressLine2 { get; set; }
    [JsonProperty("locality")]
    public string Locality { get; set; }
    [JsonProperty("city")]
    public string City { get; set; }
    [JsonProperty("county")]
    public string County { get; set; }
    [JsonProperty("postcode")]
    public string Postcode { get; set; }
}
