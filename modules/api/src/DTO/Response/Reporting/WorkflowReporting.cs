using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public class WorkflowReporting
{
    [JsonIgnore]
    public Hierarchy Hierarchy { get; set; }

    [JsonProperty("ODS_Code")]
    public string OdsCode => Hierarchy.OdsCode;

    [JsonProperty("Supplier_Name")]
    public string SupplierName { get; set; }

    [JsonProperty("Site_Name")]
    public string SiteName => Hierarchy.SiteName;

    [JsonProperty("Postcode")]
    public string Postcode => Hierarchy.Postcode;

    [JsonProperty("ICB")]
    public string Icb => string.Format("{0}{1}", Hierarchy.IcbName, Hierarchy.IcbCode != null ? $" ({Hierarchy.IcbCode})" : string.Empty);

    [JsonProperty("Higher_Health_Authority")]
    public string HigherHealthAuthority => string.Format("{0}{1}", Hierarchy.HigherHealthAuthorityName, Hierarchy.HigherHealthAuthorityCode != null ? $" ({Hierarchy.HigherHealthAuthorityCode})" : string.Empty);

    [JsonProperty("Commissioning_Region")]
    public string NationalGrouping => string.Format("{0}{1}", Hierarchy.NationalGroupingName, Hierarchy.NationalGroupingCode != null ? $" ({Hierarchy.NationalGroupingCode})" : string.Empty);

    [JsonProperty("Status")]
    public string Status { get; set; }
}