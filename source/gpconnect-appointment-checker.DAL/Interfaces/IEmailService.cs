namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IEmailService
    {
        void SendAuthorisationEmail(string recipient);
    }
}
