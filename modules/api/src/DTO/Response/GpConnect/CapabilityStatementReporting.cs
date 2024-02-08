using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class CapabilityStatementReporting
{
    public Hierarchy Hierarchy { get; set; }
    public string Version { get; set; }

    public List<Profile> Profile { get; set; }

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

    [JsonProperty("Uncategorised Data")]
    public string UncategorisedDataInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("OBSERVATION")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Investigations")]
    public string InvestigationsInProfile => (
        Profile?.Count(x => x.reference.ToUpper().Contains("DIAGNOSTICREPORT")) > 0 &&
        Profile?.Count(x => x.reference.ToUpper().Contains("SPECIMEN")) > 0
        ) ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Diary Entries")]
    public string DiaryEntriesInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("PROCEDUREREQUEST")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Referrals")]
    public string ReferralsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("REFERRALREQUEST")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Documents")]
    public string DocumentsInProfile => Profile?.Count(x => x.reference.ToUpper().Contains("DOCUMENTREFERENCE")) > 0 ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;

    [JsonProperty("Operation")]
    public IEnumerable<IEnumerable<string>> Rest { get; set; }
}

public class ActiveInactiveConstants
{
    public const string ACTIVE = "Active";
    public const string INACTIVE = "Inactive";
}