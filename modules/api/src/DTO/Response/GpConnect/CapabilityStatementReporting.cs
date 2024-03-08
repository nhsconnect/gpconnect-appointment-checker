using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Service.GpConnect;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class CapabilityStatementReporting
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

    private string _StructuredVersion;

    [JsonProperty("Structured_Version")]
    public string StructuredVersion
    {
        get { return !string.IsNullOrWhiteSpace(_StructuredVersion) ? $"v{_StructuredVersion}" : ""; }
        set { _StructuredVersion = value; }
    }

    private string _DocumentsVersion;

    [JsonProperty("Documents_Version")]
    public string DocumentsVersion
    {
        get { return !string.IsNullOrWhiteSpace(_DocumentsVersion) ? $"v{_DocumentsVersion}" : ""; }
        set { _DocumentsVersion = value; }
    }

    [JsonIgnore]
    public List<Profile> Profile { get; set; }

    [JsonIgnore]
    public List<Rest> Rest { get; set; }

    [JsonProperty("Allergies")]
    public string AllergiesInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("ALLERGYINTOLERANCE")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Medications")]
    public string MedicationsInProfile => (
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATION")) > 0 && 
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATIONSTATEMENT")) > 0 && 
        Profile?.Count(x => x.reference.ToUpper().Contains("MEDICATIONREQUEST")) > 0
        ) ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Immunisations")]
    public string ImmunisationsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("IMMUNIZATION")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Problems")]
    public string ProblemsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("PROBLEMHEADER")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Consultations")]
    public string ConsultationsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("ENCOUNTER")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Uncategorised_Data")]
    public string UncategorisedDataInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("OBSERVATION")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Investigations")]
    public string InvestigationsInProfile => (
        Profile?.Count(x => x.reference.ToUpper().Contains("DIAGNOSTICREPORT")) > 0 &&
        Profile?.Count(x => x.reference.ToUpper().Contains("SPECIMEN")) > 0
        ) ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Diary_Entries")]
    public string DiaryEntriesInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("PROCEDUREREQUEST")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Referrals")]
    public string ReferralsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("REFERRALREQUEST")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Documents")]
    public string DocumentsInProfile { get; set; }

    [JsonProperty("Operation")]
    public IEnumerable<string> Operation => Rest?.FirstOrDefault()?.Operation?.Select(x => x.Name);
}

public class ActiveInactiveConstants
{
    public const string ACTIVE = "Active";
    public const string INACTIVE = "Inactive";
}