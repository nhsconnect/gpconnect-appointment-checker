using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO
{
    public class CapabilityStatementErrorCodeOrDetail
    {
        public string SuppliedProviderOdsCode { get; set; }
        public string SuppliedConsumerOdsCode { get; set; }
        public ErrorCode errorSource { get; set; }
        public string details { get; set; }
    }
}
