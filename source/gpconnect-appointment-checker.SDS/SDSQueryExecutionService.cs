using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace gpconnect_appointment_checker.SDS
{
    public class SDSQueryExecutionService : ISDSQueryExecutionService
    {
        private readonly ILogger<SDSQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;
        private readonly LdapConnection _connection;
        private readonly IHttpContextAccessor _context;
        private X509Certificate _clientCertificate;

        public SDSQueryExecutionService(ILogger<SDSQueryExecutionService> logger, ILogService logService, IConfiguration configuration, IHttpContextAccessor context)
        {
            _logger = logger;
            _configuration = configuration;
            _logService = logService;
            _context = context;
        }

        public T ExecuteLdapQuery<T>(string searchBase, string filter) where T : class
        {
            return ExecuteLdapQuery<T>(searchBase, filter, null);
        }

        public T ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var logMessage = new SpineMessage
                {
                    RequestPayload = $"{searchBase} {filter} {attributes}",
                    SpineMessageTypeId = (int)GPConnect.Constants.SpineMessageTypes.SpineLdapQuery
                };
                var userSessionId = _context.HttpContext.User.FindFirst("UserSessionId")?.Value;
                if (userSessionId != null) logMessage.UserSessionId = Convert.ToInt32(userSessionId);

                var ldapConnection = GetConnection();
                var results = new Dictionary<string, object>();
                _logger.LogInformation("Logging immediately before the search takes place");
                _logger.LogInformation($"searchBase: {searchBase}");
                _logger.LogInformation($"filter: {filter}");
                var searchResults = ldapConnection.Search(searchBase, LdapConnection.ScopeSub, filter, attributes, false);

                _logger.LogInformation("Logging immediately after the search takes place");

                while (searchResults.HasMore())
                {
                    var nextEntry = searchResults.Next();
                    var attributeSet = nextEntry.GetAttributeSet();

                    foreach (var attribute in attributeSet)
                    {
                        results.TryAdd(attribute.Name, attribute.StringValue);
                    }
                }

                _logger.LogInformation($"Number of searchresults found: {results.Count}");

                if (results.Count > 0)
                {
                    string jsonDictionary = JsonConvert.SerializeObject(results);
                    logMessage.ResponsePayload = jsonDictionary;
                    logMessage.RoundTripTimeMs = sw.ElapsedMilliseconds;
                    _logService.AddSpineMessageLog(logMessage);

                    var result = JsonConvert.DeserializeObject<T>(jsonDictionary);
                    return result;
                }
                return null;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);


                if (exc is LdapException)
                {
                    LdapException ldapException = exc as LdapException;
                    _logger.LogError("LDAP error message: " + ldapException.LdapErrorMessage ?? string.Empty);
                    _logger.LogError("LDAP result code: " + ldapException.ResultCodeToString());

                    if (ldapException.InnerException != null)
                        _logger.LogError("Inner exception: " + ldapException.InnerException.GetType().Name + " - " + ldapException.InnerException.Message);
                }

                throw;
            }
        }

        private ILdapConnection GetConnection()
        {
            try
            {
                var ldapConn = _connection;
                var hostName = _configuration.GetSection("Spine:sds_hostname").Value;
                var hostPort = int.Parse(_configuration.GetSection("Spine:sds_port").Value);
                var useSdsMutualAuth = bool.Parse(_configuration.GetSection("Spine:sds_use_mutualauth").Value);                
                
                if (ldapConn == null && !string.IsNullOrEmpty(hostName) && hostPort > 0)
                {
                    ldapConn = new LdapConnection
                    {
                        SecureSocketLayer = bool.Parse(_configuration.GetSection("Spine:sds_use_ldaps").Value),
                        ConnectionTimeout = int.Parse(_configuration.GetSection("Spine:timeout_seconds").Value) * 1000,
                        
                    };

                    _logger.LogInformation("Initiated Ldap Connection with the following parameters");
                    _logger.LogInformation($"SecureSocketLayer: {ldapConn.SecureSocketLayer}");
                    _logger.LogInformation($"ConnectionTimeout: {ldapConn.ConnectionTimeout}");

                    if (useSdsMutualAuth)
                    {
                        _logger.LogInformation($"UseSdsMutualAuth: On");

                        var clientCert = _configuration.GetSection("spine:client_cert").GetConfigurationString();
                        _logger.LogInformation($"Retrieved Client Certificate from Database as {clientCert}");
                        var serverCert = _configuration.GetSection("spine:server_ca_certchain").GetConfigurationString();
                        _logger.LogInformation($"Retrieved Server Certificate from Database as {serverCert}");
                        var clientPrivateKey = _configuration.GetSection("spine:client_private_key").GetConfigurationString();
                        _logger.LogInformation($"Retrieved Client Private Key from Database as {clientPrivateKey}");

                        var clientCertData = CertificateHelper.ExtractCertInstances(clientCert);
                        _logger.LogInformation($"Extracted Client Certificate as Byte Array");
                        var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(clientPrivateKey);
                        _logger.LogInformation($"Extracted Client Private Key as Byte Array");
                        var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());
                        _logger.LogInformation($"Generated x509ClientCertificate using Client Certificate Byte Array");

                        var privateKey = RSA.Create();
                        _logger.LogInformation($"Created empty default empty implementation of the RSA key");
                        privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
                        _logger.LogInformation($"Imported Client Private Key byte data into RSA key");
                        var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);
                        _logger.LogInformation($"Generated x509ClientCertificate with Private Key");
                        var pfxFormattedCertificate = new X509Certificate(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);
                        _logger.LogInformation($"Generated PFX formatted Certificate of x509ClientCertificate with Private Key");

                        _clientCertificate = pfxFormattedCertificate;

                        _logger.LogInformation($"Initiating Server Cert Validation Delegate");
                        ldapConn.UserDefinedServerCertValidationDelegate += new Novell.Directory.Ldap.RemoteCertificateValidationCallback((sender, certificate, chain, errors) => ValidateServerCertificate(sender, certificate, chain, errors));
                        _logger.LogInformation($"Initiating Client Cert Selection Delegate");
                        ldapConn.UserDefinedClientCertSelectionDelegate += new Novell.Directory.Ldap.LocalCertificateSelectionCallback((sender, targetHost, certificateCollection, certificate, acceptableIssuers) => SelectLocalCertificate(sender, targetHost, certificateCollection, certificate, acceptableIssuers));
                    }
                    _logger.LogInformation($"Connecting to LDAP with the following parameters");
                    _logger.LogInformation($"Host: {hostName}");
                    _logger.LogInformation($"Port: {hostPort}");
                    ldapConn.Connect(hostName, hostPort);
                }
                return ldapConn;
            }            
            catch (LdapException ldapException)
            {
                _logger.LogError("An error has occurred while attempting to establish a connection to the LDAP server", ldapException);
                throw;
            }
        }

        public X509Certificate ValidateClientCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            _logger.LogInformation($"Certificate in collection count is {localCertificates.Count}");
            foreach (var certificate in localCertificates)
            {
                _logger.LogInformation($"Certificate in collection - subject is {certificate.Subject}");
                _logger.LogInformation($"Certificate in collection - issuer is {certificate.Issuer}");
            }

            _logger.LogInformation($"Remove Certificate - subject is {remoteCertificate.Subject}");
            _logger.LogInformation($"Remove Certificate - subject is {remoteCertificate.Issuer}");

            return remoteCertificate;
        }

        public bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            _logger.LogInformation("Certificate error: {0}", sslPolicyErrors);
            return true;
        }


        public X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
        {
            _logger.LogInformation("Client is selecting a local certificate.");
            _logger.LogInformation($"Client Cert subject is {_clientCertificate.Subject}");
            _logger.LogInformation($"Client Cert issuer is {_clientCertificate.Issuer}");
            return _clientCertificate;
            
            //if (acceptableIssuers != null &&
            //    acceptableIssuers.Length > 0 &&
            //    localCertificates != null &&
            //    localCertificates.Count > 0)
            //{
            //    // Use the first certificate that is from an acceptable issuer.
            //    foreach (X509Certificate certificate in localCertificates)
            //    {
            //        _logger.LogInformation($"Certificate in collection - subject is {certificate.Subject}");
            //        _logger.LogInformation($"Certificate in collection - issuer is {certificate.Issuer}");

            //        string issuer = certificate.Issuer;
            //        if (Array.IndexOf(acceptableIssuers, issuer) != -1)
            //            return certificate;
            //    }
            //}
            //if (localCertificates != null &&
            //    localCertificates.Count > 0)
            //    return localCertificates[0];

            //return null;
        }

        public bool ValidateServerCertificateChain(X509Certificate2 pfxFormattedCertificate, X509Chain chain, SslPolicyErrors errors)
        {            
            if(errors == SslPolicyErrors.None)
            {
                _logger.LogInformation("No SSL Policy Errors were found");
                return true;
            }
            _logger.LogInformation("SSL Policy Errors were found");
            _logger.LogInformation(errors.ToString());
            return true;
        }
    }
}
