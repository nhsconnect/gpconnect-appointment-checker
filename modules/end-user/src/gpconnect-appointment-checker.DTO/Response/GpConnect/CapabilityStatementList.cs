using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class CapabilityStatementList
    {
        public string OdsCode { get; set; }
        public CapabilityStatement CapabilityStatement { get; set; }
        public ErrorCode? ErrorCode { get; set; }
    }
}
