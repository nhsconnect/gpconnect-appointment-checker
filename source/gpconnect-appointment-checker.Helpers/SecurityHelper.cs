using System.Security.Authentication;

namespace gpconnect_appointment_checker.Helpers
{
    public static class SecurityHelper
    {
        public static SslProtocols ParseTlsVersion(string tlsVersion) =>
            tlsVersion switch
            {
                "1.0" or "Tls10" => SslProtocols.Tls,
                "1.1" or "Tls11" => SslProtocols.Tls11,
                "1.2" or "Tls12" => SslProtocols.Tls12,
                "1.3" or "Tls13" => SslProtocols.Tls13,
                _ => SslProtocols.None,
            };
    }
}
