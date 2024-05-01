using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public class AccessRecordStructuredReporting : InteractionReporting
{ 
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