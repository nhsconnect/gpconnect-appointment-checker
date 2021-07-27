namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class User
    {
        public int UserId { get; set; }
        public int UserSessionId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public string OrganisationName { get; set; }
        public string AccessLevel { get; set; }
        public string LastLogonDate { get; set; }
        public int UserAccountStatusId { get; set; }
        public bool MultiSearchEnabled { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsNewUser { get; set; }
        public int AccessRequestCount { get; set; }
        public bool IsPastLastLogonThreshold { get; set; }
        public bool StatusChanged { get; set; }
        public int OrganisationId { get; set; }
    }
}
