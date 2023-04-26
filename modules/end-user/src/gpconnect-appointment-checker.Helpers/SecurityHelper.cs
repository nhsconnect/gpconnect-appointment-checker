using System.Security.Authentication;

namespace gpconnect_appointment_checker.Helpers
{
    public static class SecurityHelper
    {
        public static SslProtocols ParseTlsVersion(string tlsVersion) =>
            tlsVersion switch
            {
                "1.3" or "Tls13" => SslProtocols.Tls13,
                _ => SslProtocols.None,
            };
    }
}
