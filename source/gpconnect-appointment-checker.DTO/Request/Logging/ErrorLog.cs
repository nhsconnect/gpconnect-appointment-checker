using System;

namespace gpconnect_appointment_checker.DTO.Request.Logging
{
    public class ErrorLog
    {
        public string Application { get; set; }
        public DateTime Logged { get; set; }
        public string Level { get; set; }
        public int UserId { get; set; }
        public int UserSessionId { get; set; }
        public string Message { get; set; }
        public string Logger { get; set; }
        public string Callsite { get; set; }
        public string Exception { get; set; }
    }
}
