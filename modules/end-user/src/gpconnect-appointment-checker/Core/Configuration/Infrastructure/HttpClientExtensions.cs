using GpConnect.AppointmentChecker.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Polly.Extensions.Http;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class HttpClientExtensions
    {
        private static readonly IConfiguration config;
        public static Spine _spineConfig => config.GetSection("Spine").Get<Spine>();        

        public static void AddHttpClientServices(IServiceCollection services, IWebHostEnvironment env)
        {
            Action<HttpClient> httpClientConfig = options =>
            {
                options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
                options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            };

            //services.AddHttpClient("GpConnectClient", options =>
            //{
            //    options.Timeout = new TimeSpan(0, 0, 0, _spineConfig.TimeoutSeconds);
            //    options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/fhir+json"));
            //    options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
            //}).ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(env)).SetHandlerLifetime(System.Threading.Timeout.InfiniteTimeSpan);
        }

        //private static IHttpClientBuilder AugmentHttpClientBuilder(this IHttpClientBuilder httpClientBuilder, IWebHostEnvironment env)
        //{
        //    return httpClientBuilder.AddPolicyHandler(GetRetryPolicy())
        //        .SetHandlerLifetime(TimeSpan.FromMinutes(5))
        //        .ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(env));
        //}

        //private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        //{
        //    return HttpPolicyExtensions
        //        .HandleTransientHttpError()
        //        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));
        //}

        //private static HttpMessageHandler CreateHttpMessageHandler(IWebHostEnvironment env)
        //{
        //    var httpClientHandler = new HttpClientHandler() { MaxConnectionsPerServer = 1 };

        //    if (_spineConfig.UseSSP)
        //    {
        //        httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12 | SslProtocols.Tls11 | SslProtocols.Tls;

        //        var clientCertData = CertificateHelper.ExtractCertInstances(_spineConfig.ClientCert);
        //        var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(_spineConfig.ClientPrivateKey);
        //        var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());
        //        var privateKey = RSA.Create();
        //        privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
        //        var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
        //        var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

        //        var serverCertData = CertificateHelper.ExtractCertInstances(_spineConfig.ServerCACertChain);
        //        var x509ServerCertificateSubCa = new X509Certificate2(serverCertData[0]);
        //        var x509ServerCertificateRootCa = new X509Certificate2(serverCertData[1]);

        //        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => ValidateServerCertificateChain(chain, x509ServerCertificateSubCa, x509ServerCertificateRootCa, pfxFormattedCertificate);

        //        httpClientHandler.ClientCertificates.Add(pfxFormattedCertificate);
        //        httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;
        //        return httpClientHandler;
        //    }
        //    return httpClientHandler;
        //}

        //private static bool ValidateServerCertificateChain(X509Chain chain, X509Certificate2 x509ServerCertificateSubCa,
        //    X509Certificate2 x509ServerCertificateRootCa, X509Certificate2 pfxFormattedCertificate)
        //{
        //    chain.Reset();
        //    chain.ChainPolicy.VerificationFlags = X509VerificationFlags.IgnoreRootRevocationUnknown;
        //    chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
        //    chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateSubCa);
        //    chain.ChainPolicy.ExtraStore.Add(x509ServerCertificateRootCa);

        //    if (chain.Build(pfxFormattedCertificate)) return true;
        //    if (chain.ChainStatus.Where(chainStatus => chainStatus.Status != X509ChainStatusFlags.NoError).All(chainStatus => chainStatus.Status != X509ChainStatusFlags.UntrustedRoot)) return false;
        //    var providedRoot = chain.ChainElements[^1];
        //    return x509ServerCertificateRootCa.Thumbprint == providedRoot.Certificate.Thumbprint;
        //}
    }
}
