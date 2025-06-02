namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Lock
{
    public string Resource { get; set; } = null!;

    public int Updatecount { get; set; }

    public DateTime? Acquired { get; set; }
}
