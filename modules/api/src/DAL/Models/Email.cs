namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Email
{
    public bool SingleRowLock { get; set; }

    public string SenderAddress { get; set; } = null!;

    public string HostName { get; set; } = null!;

    public short Port { get; set; }

    public string Encryption { get; set; } = null!;

    public string UserName { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string DefaultSubject { get; set; } = null!;
}
