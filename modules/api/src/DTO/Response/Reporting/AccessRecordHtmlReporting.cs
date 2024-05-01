using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Newtonsoft.Json;

namespace gpconnect_appointment_checker.api.DTO.Response.Reporting;

public class AccessRecordHtmlReporting : InteractionReporting
{    
    [JsonProperty("Status")]
    public string Status => Rest != null && Rest.Any(x => x.Operation != null) ? ActiveInactiveConstants.ACTIVE : ActiveInactiveConstants.INACTIVE;
}