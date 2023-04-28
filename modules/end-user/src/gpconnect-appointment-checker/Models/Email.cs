using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Models;

public class Email
{
    [JsonProperty("senderAddress")]
    public string SenderAddress { get; set; }

    [JsonProperty("hostName")]
    public string HostName { get; set; }

    [JsonProperty("port")]
    public int Port { get; set; }

    [JsonProperty("encryption")]
    public int Encryption { get; set; }

    [JsonProperty("userName")]
    public string UserName { get; set; }
    
    [JsonProperty("password")]
    public string Password { get; set; }

    [JsonProperty("defaultSubject")]
    public string DefaultSubject { get; set; }
}
