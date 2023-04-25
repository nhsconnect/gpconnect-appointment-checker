using System.Security.Authentication;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

public class Email
{
    public string SenderAddress { get; set; }
    public string HostName { get; set; }
    public int Port { get; set; }
    public SslProtocols Encryption { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DefaultSubject { get; set; }
}
