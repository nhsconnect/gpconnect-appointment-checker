using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotSummary
    {
        public List<SlotEntrySummary> SlotEntrySummary { get; set; }
        public List<Issue> Issue { get; set; }
    }
}
