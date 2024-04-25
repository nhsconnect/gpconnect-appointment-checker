using GpConnect.AppointmentChecker.Api.Helpers;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GpConnect.AppointmentChecker.Api.Core.HttpClientServices;

public static class HttpClientExtensions
{
    public static void AddHttpClientServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        GetHttpClient(services, configuration, env, Helpers.Constants.Clients.FHIRREADCLIENT, false);
        GetHttpClient(services, configuration, env, Helpers.Constants.Clients.HIERARCHYCLIENT, false);
        GetHttpClient(services, configuration, env, Helpers.Constants.Clients.GPCONNECTCLIENT, true);
        GetMeshClient(services, configuration, env);
    }

    private static void GetMeshClient(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        GetHttpClient(services, configuration, env, Helpers.Constants.Clients.MESHCLIENT, true, "application/vnd.mesh.v2+json");
    }

    private static void GetHttpClient(IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env, string clientName, bool handleSSP, string mediaType = "application/fhir+json")
    {
        services.AddHttpClient(clientName, options =>
        {
            options.Timeout = TimeSpan.FromSeconds(60);
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }).AugmentHttpClientBuilder(env, configuration, handleSSP);
    }

    private static IHttpClientBuilder AugmentHttpClientBuilder(this IHttpClientBuilder httpClientBuilder, IWebHostEnvironment env, IConfiguration configuration, bool handleSSP)
    {
        return httpClientBuilder.AddPolicyHandler(GetRetryPolicy())
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler(env, configuration, handleSSP));
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));
    }

    private static HttpMessageHandler CreateHttpMessageHandler(IWebHostEnvironment env, IConfiguration configuration, bool handleSSP)
    {
        var httpClientHandler = new HttpClientHandler();

        if(httpClientHandler.SupportsAutomaticDecompression)
        {
            httpClientHandler.AutomaticDecompression = System.Net.DecompressionMethods.Deflate | System.Net.DecompressionMethods.GZip;
        }

        if (handleSSP)
        {
            var spineConfig = configuration.GetSection("SpineConfig");

            if (spineConfig.GetValue<bool>("UseSSP"))
            {
                httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;

                var clientCertData = CertificateHelper.ExtractCertInstances(spineConfig.GetValue<string>("ClientCert"));
                var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(spineConfig.GetValue<string>("ClientPrivateKey"));
                var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());
                var privateKey = RSA.Create();
                privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
                var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
                var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

                var serverCertData = CertificateHelper.ExtractCertInstances(spineConfig.GetValue<string>("ServerCACertChain"));
                var x509ServerCertificateSubCa = new X509Certificate2(serverCertData[0]);
                var x509ServerCertificateRootCa = new X509Certificate2(serverCertData[1]);

                httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => ValidateServerCertificateChain(chain, x509ServerCertificateSubCa, x509ServerCertificateRootCa, pfxFormattedCertificate);

                httpClientHandler.ClientCertificates.Add(pfxFormattedCertificate);
                httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

                var clientCertExpirationDate = x509ClientCertificate.GetExpirationDateString();
                var subCaCertExpirationDate = x509ServerCertificateSubCa.GetExpirationDateString();
                var rootCaCertExpirationDate = x509ServerCertificateRootCa.GetExpirationDateString();
            }
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
}
