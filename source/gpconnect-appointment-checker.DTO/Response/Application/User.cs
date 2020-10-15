using System;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class User
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string DisplayName { get; set; }
        public int OrganisationId { get; set; }
        public int UserSessionId { get; set; }
        public bool IsAuthorised { get; set; }
        public DateTime AddedDate { get; set; }
        public DateTime AuthorisedDate { get; set; }
        public DateTime LastLogonDate { get; set; }
    }
}
