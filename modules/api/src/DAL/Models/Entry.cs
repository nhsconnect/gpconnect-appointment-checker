namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Entry
{
    public int EntryId { get; set; }

    public int? UserId { get; set; }

    public int? UserSessionId { get; set; }

    public short EntryTypeId { get; set; }

    public string? Item1 { get; set; }

    public string? Item2 { get; set; }

    public string? Item3 { get; set; }

    public string? Details { get; set; }

    public int? EntryElapsedMs { get; set; }

    public DateTime EntryDate { get; set; }

    public int? AdminUserId { get; set; }

    public virtual User? AdminUser { get; set; }

    public virtual EntryType EntryType { get; set; } = null!;

    public virtual User? User { get; set; }

    public virtual UserSession? UserSession { get; set; }
}
