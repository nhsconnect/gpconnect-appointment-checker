﻿using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Reflection.Metadata.Ecma335;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

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
            var serverCert = configuration.GetSection("spine:server_ca_certchain").GetConfigurationString();
            var clientPrivateKey = configuration.GetSection("spine:client_private_key").GetConfigurationString();

            var httpClientHandler = new HttpClientHandler();

            if (bool.Parse(configuration.GetSection("spine:use_ssp").GetConfigurationString("false")) &&
                !string.IsNullOrEmpty(clientCert) && !string.IsNullOrEmpty(clientPrivateKey) &&
                !string.IsNullOrEmpty(configuration.GetSection("spine:nhsMHSEndPoint").GetConfigurationString()))
            {
                httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

                var clientCertData = ExtractCertInstances(clientCert);
                var clientPrivateKeyData = ExtractKeyInstance(clientPrivateKey);
                var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());
                var privateKey = RSA.Create();
                privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
                var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
                var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

                var serverCertData = ExtractCertInstances(serverCert);
                var x509ServerCertificateSubCa = new X509Certificate2(serverCertData[0]);
                var x509ServerCertificateRootCa = new X509Certificate2(serverCertData[1]);

                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => ValidateServerCertificateChain(chain, x509ServerCertificateSubCa, x509ServerCertificateRootCa, pfxFormattedCertificate);

                httpClientHandler.ClientCertificates.Add(pfxFormattedCertificate);
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
                return httpClientHandler;
            }
            return httpClientHandler;
        }

        private static bool ValidateServerCertificateChain(X509Chain chain, X509Certificate2 x509ServerCertificateSubCa,
            X509Certificate2 x509ServerCertificateRootCa, X509Certificate2 pfxFormattedCertificate)
        {
            chain.Reset();
            chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreRootRevocationUnknown;
            chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
            chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateSubCa);
            chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateRootCa);

            if (chain.Build(pfxFormattedCertificate)) return true;
            if (chain.ChainStatus.Where(chainStatus => chainStatus.Status != X509ChainStatusFlags.NoError).All(chainStatus => chainStatus.Status != X509ChainStatusFlags.UntrustedRoot)) return false;
            var providedRoot = chain.ChainElements[^1];
            return x509ServerCertificateRootCa.Thumbprint == providedRoot.Certificate.Thumbprint;
        }

        private static List<byte[]> ExtractCertInstances(string certData)
        {
            const string BEGIN_CERTIFICATE = "-----BEGIN CERTIFICATE-----";
            const string END_CERTIFICATE = "-----END CERTIFICATE-----";

            var matchesFound = Regex.Matches(certData, BEGIN_CERTIFICATE + "(.+?)" + END_CERTIFICATE, RegexOptions.Singleline)
                .Select(m => Convert.FromBase64String(m.Groups[1].Value))
                .ToList();
            return matchesFound;
        }

        private static byte[] ExtractKeyInstance(string keyData)
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
