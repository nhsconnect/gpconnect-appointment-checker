using System.Collections.Generic;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotSimple
    {
        public List<SlotEntrySimple> SlotEntrySimple { get; set; }
        public List<Issue> Issue { get; set; }
    }
}
