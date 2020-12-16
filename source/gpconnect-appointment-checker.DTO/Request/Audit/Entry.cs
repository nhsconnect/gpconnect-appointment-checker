using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Request.Audit
{
    public class Entry
    {
        public int EntryTypeId { get; set; }
        public string Item1 { get; set; }
        public string Item2 { get; set; }
        public string Item3 { get; set; }
        public string Details { get; set; }
        public int EntryElapsedMs { get; set; }
    }
}
