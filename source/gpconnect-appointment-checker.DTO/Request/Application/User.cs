namespace gpconnect_appointment_checker.DTO.Request.Application
{
    public class User
    {
        public int OrganisationId { get; set; }
        public int UserSessionId { get; set; }
        public string DisplayName { get; set; }
        public string EmailAddress { get; set; }
        public bool IsAuthorised { get; set; }
        public bool IsAdmin { get; set; }
        public bool MultiSearchEnabled { get; set; }
        public bool OrgTypeSearchEnabled { get; set; }
    }
}
