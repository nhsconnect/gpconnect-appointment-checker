using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public class AccessRecordStructuredReporting
{
    [JsonIgnore]
    public Hierarchy Hierarchy { get; set; }

    [JsonProperty("ODS_Code")]
    public string OdsCode { get; set; }

    [JsonProperty("Supplier_Name")]
    public string SupplierName { get; set; }

    [JsonProperty("Site_Name")]
    public string SiteName => Hierarchy != null ? Hierarchy.SiteName : string.Empty;

    [JsonProperty("Postcode")]
    public string Postcode => Hierarchy != null ? Hierarchy.Postcode : string.Empty;

    [JsonProperty("ICB")]
    public string Icb => Hierarchy != null ? string.Format("{0}{1}", Hierarchy.IcbName, Hierarchy.IcbCode != null ? $" ({Hierarchy.IcbCode})" : string.Empty) : string.Empty;

    [JsonProperty("Higher_Health_Authority")]
    public string HigherHealthAuthority => Hierarchy != null ? string.Format("{0}{1}", Hierarchy?.HigherHealthAuthorityName, Hierarchy.HigherHealthAuthorityCode != null ? $" ({Hierarchy.HigherHealthAuthorityCode})" : string.Empty) : string.Empty;

    [JsonProperty("Commissioning_Region")]
    public string NationalGrouping => Hierarchy != null ? string.Format("{0}{1}", Hierarchy.NationalGroupingName, Hierarchy.NationalGroupingCode != null ? $" ({Hierarchy.NationalGroupingCode})" : string.Empty) : string.Empty;

    [JsonProperty("Version")]
    public string ApiVersion { get; set; }

    [JsonIgnore]
    public List<Rest>? Rest { get; set; }

    [JsonProperty("Operation")]
    public IEnumerable<string> Operation => Rest?.FirstOrDefault()?.Operation?.Select(x => x.Name);

    [JsonIgnore]
    public List<Profile>? Profile { get; set; }    

    [JsonProperty("Allergies")]
    public string AllergiesInProfile => CheckProfileSegment("ALLERGYINTOLERANCE");

    private string CheckProfileSegment(string profileValue)
    {
        return Profile != null ? Profile?.Count(x => x.reference.ToUpper().Contains(profileValue)) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE : ActiveInactiveConstants.NOTAVAILABLE;
    }

    [JsonProperty("Medications")]
    public string MedicationsInProfile => Profile != null ? 
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATION")) > 0 &&
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATIONSTATEMENT")) > 0 &&
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATIONREQUEST")) > 0
         ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE : ActiveInactiveConstants.NOTAVAILABLE;

    [JsonProperty("Immunisations")]
    public string ImmunisationsInProfile => CheckProfileSegment("IMMUNIZATION");

    [JsonProperty("Problems")]
    public string ProblemsInProfile => CheckProfileSegment("PROBLEMHEADER");

    [JsonProperty("Consultations")]
    public string ConsultationsInProfile => CheckProfileSegment("ENCOUNTER");

    [JsonProperty("Uncategorised_Data")]
    public string UncategorisedDataInProfile => CheckProfileSegment("OBSERVATION");

    [JsonProperty("Investigations")]
    public string InvestigationsInProfile => Profile != null ? 
        Profile?.Count(x => x.reference.ToUpper().Contains("DIAGNOSTICREPORT")) > 0 &&
        Profile?.Count(x => x.reference.ToUpper().Contains("SPECIMEN")) > 0
         ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE : ActiveInactiveConstants.NOTAVAILABLE;

    [JsonProperty("Diary_Entries")]
    public string DiaryEntriesInProfile => CheckProfileSegment("PROCEDUREREQUEST");

    [JsonProperty("Referrals")]
    public string ReferralsInProfile => CheckProfileSegment("REFERRALREQUEST");    

    [JsonProperty("Documents")]
    public string DocumentsInProfile { get; set; }

    [JsonProperty("Documents_Version")]
    public string DocumentsVersion { get; set; }
}