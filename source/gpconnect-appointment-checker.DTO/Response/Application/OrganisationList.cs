using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class OrganisationList
    {
        public string OdsCode { get; set; }
        public Organisation Organisation { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public double TimeTakenInSeconds { get; set; }
    }
}
