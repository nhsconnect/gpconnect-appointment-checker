using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO
{
    public class OrganisationErrorCodeOrDetail
    {
        public string SuppliedProviderOdsCode { get; set; }
        public string SuppliedConsumerOdsCode { get; set; }
        public ErrorCode errorSource { get; set; }
        public string details { get; set; }
        public Organisation providerOrganisation { get; set; }
        public Organisation consumerOrganisation { get; set; }
        public Spine providerSpine { get; set; }
    }
}
