using System;

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
        public DateTime LastLogonDate { get; set; }
        public string Status { get; set; }
        public bool IsAuthorised { get; set; }
        public bool MultiSearchEnabled { get; set; }
        public bool IsAdmin { get; set; }
    }
}
