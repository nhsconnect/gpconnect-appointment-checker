using System.Collections.Generic;
using System.IO;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotSimple
    {
        public List<SlotEntrySimple> SlotEntrySimple { get; set; }
        public List<Issue> Issue { get; set; }
        public MemoryStream SlotEntrySimpleAsStream { get; set; }
    }
}
