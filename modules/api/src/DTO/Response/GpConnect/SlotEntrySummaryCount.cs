using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class SlotEntrySummaryCount
{
    public string OdsCode { get; set; }
    public ErrorCode ErrorCode { get; set; }
    public List<Issue> ErrorDetail { get; set; }
    public int? FreeSlotCount { get; set; }
    public int? SpineMessageId { get; set; }
}
