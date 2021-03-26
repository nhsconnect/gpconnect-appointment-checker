namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IEmailService
    {
        void SendUserStatusEmail(bool isAuthorised, string recipient);
    }
}
