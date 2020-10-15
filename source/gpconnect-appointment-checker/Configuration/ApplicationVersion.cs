using System.Reflection;

namespace gpconnect_appointment_checker.Configuration
{
    public static class ApplicationVersion
    {
        public static string GetAssemblyVersion => Assembly.GetEntryAssembly()?.GetName().FullName;
    }
}
