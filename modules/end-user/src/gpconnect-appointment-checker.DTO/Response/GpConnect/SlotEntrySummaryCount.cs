using System.Collections.Generic;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DTO.Response.GpConnect
{
    public class SlotEntrySummaryCount
    {
        public string OdsCode { get; set; }
        public ErrorCode ErrorCode { get; set; }
        public List<Issue> ErrorDetail { get; set; }
        public int? FreeSlotCount { get; set; }
        public int? SpineMessageId { get; set; }
    }
}
