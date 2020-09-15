namespace gpconnect_appointment_checker.Logging.GlobalExceptionHandler
{
    public interface ILog
    {
        void Information(string message);
        void Warning(string message);
        void Debug(string message);
        void Error(string message);
    }
}
