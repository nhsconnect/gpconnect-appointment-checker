using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class HttpClientExtensions
    {
        public static void AddHttpClientServices(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddHttpClient("GpConnectClient", options =>
            {
                options.Timeout = new TimeSpan(0, 0, 0, int.Parse(configuration.GetSection("spine:timeout_seconds").GetConfigurationString("30")));
                options.DefaultRequestHeaders.Add("Accept", "application/fhir+json");
                options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            }).ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(configuration, env));
        }

        private static HttpMessageHandler CreateHttpMessageHandler(IConfiguration configuration, IWebHostEnvironment env)
        {
            var clientCert = configuration.GetSection("spine:client_cert").GetConfigurationString();
            var clientPrivateKey = configuration.GetSection("spine:client_private_key").GetConfigurationString();

            var httpClientHandler = new HttpClientHandler();

            if (bool.Parse(configuration.GetSection("spine:use_ssp").GetConfigurationString("false")) &&
                !string.IsNullOrEmpty(clientCert) && !string.IsNullOrEmpty(clientPrivateKey) &&
                !string.IsNullOrEmpty(configuration.GetSection("spine:nhsMHSEndPoint").GetConfigurationString()))
            {
                httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;
                httpClientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => env.IsDevelopment();

                var x509Certificate = new X509Certificate2(Convert.FromBase64String(clientCert));
                var privateKey = RSA.Create();
                privateKey.ImportRSAPrivateKey(Convert.FromBase64String(clientPrivateKey), out _);
                var x509CertificateWithPrivateKey = x509Certificate.CopyWithPrivateKey(privateKey);

                var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

                httpClientHandler.ClientCertificates.Add(pfxFormattedCertificate);
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                return httpClientHandler;
            }
            return httpClientHandler;
        }
    }
}
