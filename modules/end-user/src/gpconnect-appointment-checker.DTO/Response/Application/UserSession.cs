using System;

namespace gpconnect_appointment_checker.DTO.Response.Application
{
    public class UserSession
    {
        public int UserSessionId { get; set; }
        public int UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
    }
}
