namespace GpConnect.AppointmentChecker.Api.Dal.Configuration;

public class ConnectionStrings
{
    public ConnectionStrings()
    {
        DefaultConnection = "DefaultConnection";
    }

    public string DefaultConnection { get; set; }
}