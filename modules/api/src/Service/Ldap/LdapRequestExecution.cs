using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.Helpers;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System.Diagnostics;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;

public class LdapRequestExecution : ILdapRequestExecution
{
    private static ILogger<LdapRequestExecution> _logger;
    private static X509Certificate _clientCertificate;
    private readonly ILogService _logService;
    private static bool _haveLoggedTlsVersion = false;
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;

    public LdapRequestExecution(ILogger<LdapRequestExecution> logger, ILogService logService, IOptions<SpineConfig> spineOptionsDelegate)
    {
        _logger = logger;
        _logService = logService;
        _spineOptionsDelegate = spineOptionsDelegate;
    }

    public T ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class
    {
        try
        {
            var sw = new Stopwatch();
            T result = null;
            sw.Start();
            var logMessage = new SpineMessage
            {
                RequestPayload = $"{searchBase} {filter} [{string.Join(",", attributes)}]",
                SpineMessageTypeId = (int)Helpers.Constants.SpineMessageTypes.SpineLdapQuery
            };

            var results = new Dictionary<string, object>();

            var ldapConnectionOptions = new LdapConnectionOptions();

            if (_spineOptionsDelegate.Value.SdsUseLdaps)
            {
                ldapConnectionOptions.ConfigureSslProtocols(SecurityHelper.ParseTlsVersion(_spineOptionsDelegate.Value.SdsTlsVersion));
                ldapConnectionOptions.UseSsl();
                ldapConnectionOptions.ConfigureLocalCertificateSelectionCallback(SelectLocalCertificate);
                ldapConnectionOptions.ConfigureRemoteCertificateValidationCallback(ValidateServerCertificate);
            }

            using (var ldapConnection = new LdapConnection(ldapConnectionOptions)
            {
                ConnectionTimeout = _spineOptionsDelegate.Value.TimeoutMilliseconds
            })
            {
                SetupMutualAuth();
                
                ldapConnection.Connect(_spineOptionsDelegate.Value.SdsHostname, _spineOptionsDelegate.Value.SdsPort);
                ldapConnection.Bind(string.Empty, string.Empty);

                LogTlsVersionOnStartup(ldapConnection);

                var searchResults = ldapConnection.Search(searchBase, LdapConnection.ScopeSub, filter, attributes, false);

                while (searchResults.HasMore())
                {
                    var nextEntry = searchResults.Next();
                    var attributeSet = nextEntry.GetAttributeSet();

                    foreach (var attribute in attributeSet)
                    {
                        results.TryAdd(attribute.Name, attribute.StringValue);
                    }
                }

                ldapConnection.Disconnect();
                ldapConnection.Dispose();
            }

            var jsonDictionary = JsonConvert.SerializeObject(results);
            if (results.Count > 0)
            {
                result = JsonConvert.DeserializeObject<T>(jsonDictionary);
            }
            logMessage.ResponsePayload = jsonDictionary;
            logMessage.RoundTripTimeMs = sw.Elapsed.TotalMilliseconds;
            _logService.AddSpineMessageLog(logMessage);
            return result;

        }
        catch (LdapException ldapException)
        {
            _logger.LogError(ldapException, "An LdapException has occurred while attempting to execute an LDAP query");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An Exception has occurred while attempting to execute an LDAP query");
            throw;
        }
    }

    private static void LogTlsVersionOnStartup(LdapConnection ldapConnection)
    {
        if (_haveLoggedTlsVersion)
            return;

        try
        {
            var tlsVersion = GetTlsVersionInUse(ldapConnection);
        }
        finally
        {
            _haveLoggedTlsVersion = true;
        }
    }

    private static string GetTlsVersionInUse(LdapConnection ldapConnection)
    {
        try
        {
            System.Reflection.PropertyInfo propertyInfo = typeof(LdapConnection).GetProperty("Connection", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var connection = propertyInfo.GetValue(ldapConnection);

            System.Reflection.FieldInfo fieldInfo = connection.GetType().GetField("_inStream", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            System.IO.Stream stream = (System.IO.Stream)fieldInfo.GetValue(connection);

            SslStream sslStream = stream as SslStream;

            if (sslStream == null)
                return "TLS not enabled";

            return sslStream.SslProtocol.ToString();
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error getting LDAP TLS version");
            return "Error getting LDAP TLS version";
        }
    }

    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
            return true;

        if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
            return true;

        _logger.LogError($"An error has occurred while attempting to validate the LDAP server certificate: {sslPolicyErrors}");
        return true;
    }

    private void SetupMutualAuth()
    {
        if (_spineOptionsDelegate.Value.SdsUseMutualAuth)
        {
            var clientCertData = CertificateHelper.ExtractCertInstances(_spineOptionsDelegate.Value.ClientCert);
            var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(_spineOptionsDelegate.Value.ClientPrivateKey);
            var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());

            var privateKey = RSA.Create();
            privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
            var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
            var pfxFormattedCertificate = new X509Certificate(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

            _clientCertificate = pfxFormattedCertificate;
        }
    }

    private static X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
    {
        return _clientCertificate;
    }
}
