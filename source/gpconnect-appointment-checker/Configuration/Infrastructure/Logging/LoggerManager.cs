using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using NLog;

namespace gpconnect_appointment_checker.Configuration
{
    public class LoggerManager : ILoggerManager
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public LoggerManager(IHttpContextAccessor context)
        {
            LogManager.Configuration.Variables.Add("userSessionId", context.HttpContext.User.GetClaimValue("UserSessionId", nullIfEmpty: true));
            LogManager.Configuration.Variables.Add("userId", context.HttpContext.User.GetClaimValue("UserId", nullIfEmpty: true));
        }

        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogInformation(string message)
        {
            logger.Info(message);
        }
        public void LogWarning(string message)
        {
            logger.Warn(message);
        }
    }
}
