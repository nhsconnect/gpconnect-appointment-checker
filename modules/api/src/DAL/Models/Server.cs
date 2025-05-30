namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Server
{
    public string Id { get; set; } = null!;

    public string? Data { get; set; }

    public DateTime Lastheartbeat { get; set; }

    public int Updatecount { get; set; }
}
