using GpConnect.AppointmentChecker.Api.Helpers;
using Polly;
using Polly.Extensions.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GpConnect.AppointmentChecker.Api.Core.HttpClientServices;

public static class MeshClientExtensions
{
    private static IConfigurationSection _meshConfig;

    public static void AddMeshClientServices(this IServiceCollection services, IConfiguration configuration)
    {
        _meshConfig = configuration.GetSection("MeshConfig");
        GetMeshClient(services);
    }

    private static void GetMeshClient(IServiceCollection services)
    {
        services.AddHttpClient(Helpers.Constants.Clients.MESHCLIENT, options =>
        {
            var uriBuilder = new UriBuilder(_meshConfig.GetValue<string>("MeshHostname"));
            uriBuilder.Path = _meshConfig.GetValue<string>("EndpointAddress");

            options.Timeout = TimeSpan.FromSeconds(60);
            options.BaseAddress = uriBuilder.Uri;
            options.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.mesh.v2+json"));
            options.DefaultRequestHeaders.CacheControl = new CacheControlHeaderValue { NoCache = true };
        }).AugmentHttpClientBuilder();
    }

    private static IHttpClientBuilder AugmentHttpClientBuilder(this IHttpClientBuilder httpClientBuilder)
    {
        return httpClientBuilder.AddPolicyHandler(GetRetryPolicy())
            .SetHandlerLifetime(Timeout.InfiniteTimeSpan)
            .ConfigurePrimaryHttpMessageHandler(() => CreateHttpMessageHandler());
    }

    private static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(1));
    }

    private static HttpMessageHandler CreateHttpMessageHandler()
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.SslProtocols = SslProtocols.Tls13 | SslProtocols.Tls12;

        var clientCertData = CertificateHelper.ExtractCertInstances(_meshConfig.GetValue<string>("ClientCert"));
        var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(_meshConfig.GetValue<string>("ClientPrivateKey"));
        var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());
        var privateKey = RSA.Create();
        privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
        var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
        var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

        var serverCertData = CertificateHelper.ExtractCertInstances(_meshConfig.GetValue<string>("ServerCACertChain"));
        var x509ServerCertificateSubCa = new X509Certificate2(serverCertData[0]);
        var x509ServerCertificateRootCa = new X509Certificate2(serverCertData[1]);

        httpClientHandler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => ValidateServerCertificateChain(chain, x509ServerCertificateSubCa, x509ServerCertificateRootCa, pfxFormattedCertificate);

        httpClientHandler.ClientCertificates.Add(pfxFormattedCertificate);
        httpClientHandler.ClientCertificateOptions = ClientCertificateOption.Manual;

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
