namespace GpConnect.AppointmentChecker.Api.DTO.Request.Audit;

public class Entry
{
    public int UserId { get; set; }
    public int EntryTypeId { get; set; }
    public string Item1 { get; set; }
    public string Item2 { get; set; }
    public string Item3 { get; set; }
    public string Details { get; set; }
    public int EntryElapsedMs { get; set; }
}
