namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SearchExport
{
    public int SearchExportId { get; set; }

    public int UserId { get; set; }

    public string SearchExportData { get; set; } = null!;

    public DateTime CreatedDate { get; set; }

    public virtual User User { get; set; } = null!;
}
