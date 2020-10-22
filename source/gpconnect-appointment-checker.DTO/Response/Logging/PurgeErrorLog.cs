using System;

namespace gpconnect_appointment_checker.DTO.Response.Logging
{
    public class PurgeErrorLog
    {
        public int ErrorLogDeletedCount { get; set; }
        public int SpineMessageDeletedCount { get; set; }
        public int WebRequestDeletedCount { get; set; }
    }
}
