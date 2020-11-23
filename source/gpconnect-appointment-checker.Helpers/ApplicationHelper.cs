using System.Reflection;

namespace gpconnect_appointment_checker.Helpers
{
    public class ApplicationHelper
    {
        public static class ApplicationVersion
        {
            public static string GetAssemblyVersion => Assembly.GetEntryAssembly()?.GetName().FullName;
        }
    }
}
