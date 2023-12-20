using Newtonsoft.Json;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models;

public class OrganisationSpine
{
    [JsonProperty("organisationId")]
    public int OrganisationId { get; set; }
    [JsonProperty("odsCode")]
    public string OdsCode { get; set; }
    [JsonProperty("organisationName")]
    public string OrganisationName { get; set; }
    [JsonProperty("postalAddress")]
    public string PostalAddress { get; set; }
    [JsonProperty("postalAddressFields")]
    public List<string> PostalAddressFields { get; set; }
    [JsonProperty("postalCode")]
    public string PostalCode { get; set; }
    [JsonProperty("organisationTypeCode")]
    public string OrganisationTypeCode { get; set; }
    [JsonProperty("formattedOrganisationDetails")]
    public string FormattedOrganisationDetails { get; set; }
    [JsonProperty("organisationLocation")]
    public string OrganisationLocation { get; set; }
}
