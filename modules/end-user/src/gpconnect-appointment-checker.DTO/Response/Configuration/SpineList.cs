using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO.Response.Configuration
{
    public class SpineList
    {
        public string OdsCode { get; set; }
        public string PartyKey { get; set; }
        //public Spine Spine { get; set; }
        //public bool ProviderEnabledForGpConnectAppointmentManagement => Spine != null;
        //public bool ProviderAsIdPresent => Spine != null && !string.IsNullOrEmpty(Spine.AsId);
        public ErrorCode? ErrorCode { get; set; }
        public double TimeTakenInSeconds { get; set; }
    }
}
