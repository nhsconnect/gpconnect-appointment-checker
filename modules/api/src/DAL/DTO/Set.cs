namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Set
{
    public long Id { get; set; }

    public string Key { get; set; } = null!;

    public double Score { get; set; }

    public string Value { get; set; } = null!;

    public DateTime? Expireat { get; set; }

    public int Updatecount { get; set; }
}
