using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Function.DTO.Response;

public class OrganisationHierarchy
{
    [JsonProperty("odsCode")]
    public string OdsCode { get; set; }

    [JsonProperty("siteName")]
    public string SiteName { get; set; }

    [JsonProperty("postcode")]
    public string Postcode { get; set; }

    [JsonProperty("icbCode")]
    public string IcbCode { get; set; }

    [JsonProperty("higherHealthAuthorityCode")]
    public string HigherHealthAuthorityCode { get; set; }

    [JsonProperty("nationalGroupingCode")]
    public string NationalGroupingCode { get; set; }

    [JsonProperty("icbName")]
    public string IcbName { get; set; }

    [JsonProperty("higherHealthAuthorityName")]
    public string HigherHealthAuthorityName { get; set; }

    [JsonProperty("nationalGroupingName")]
    public string NationalGroupingName { get; set; }
}
