namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Transient
{
    public string TransientId { get; set; } = null!;

    public string TransientData { get; set; } = null!;

    public string TransientReportId { get; set; } = null!;

    public string TransientReportName { get; set; } = null!;

    public DateTime EntryDate { get; set; }
}
