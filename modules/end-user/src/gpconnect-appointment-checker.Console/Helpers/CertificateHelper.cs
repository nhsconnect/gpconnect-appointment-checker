using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace gpconnect_appointment_checker.Console.Helpers
{
    public static class CertificateHelper
    {
        public static List<byte[]> ExtractCertInstances(string certData)
        {
            const string BEGIN_CERTIFICATE = "-----BEGIN CERTIFICATE-----";
            const string END_CERTIFICATE = "-----END CERTIFICATE-----";

            var matchesFound = Regex.Matches(certData, BEGIN_CERTIFICATE + "(.+?)" + END_CERTIFICATE, RegexOptions.Singleline)
                .Select(m => Convert.FromBase64String(m.Groups[1].Value))
                .ToList();
            return matchesFound;
        }

        public static byte[] ExtractKeyInstance(string keyData)
        {
            const string BEGIN_KEY = "-----BEGIN RSA PRIVATE KEY-----";
            const string END_KEY = "-----END RSA PRIVATE KEY-----";

            var matchesFound = Regex.Matches(keyData, BEGIN_KEY + "(.+?)" + END_KEY, RegexOptions.Singleline)
                .SelectMany(m => Convert.FromBase64String(m.Groups[1].Value))
                .ToArray();
            return matchesFound;
        }
    }
}
