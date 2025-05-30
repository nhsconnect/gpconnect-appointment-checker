namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class List1
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public string? Value { get; set; }

    public DateTime? Expireat { get; set; }

    public int Updatecount { get; set; }
}
