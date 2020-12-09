using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
//using LdapForNet;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
//using static LdapForNet.Native.Native;
using Novell.Directory.Ldap;

namespace gpconnect_appointment_checker.SDS
{
    public class SDSQueryExecutionService : ISDSQueryExecutionService
    {
        private static ILogger<SDSQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private static IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;
        private static X509Certificate _clientCertificate;
        //LDAP4NET
        //private static X509Certificate2 _clientCertificate;

        public SDSQueryExecutionService(ILogger<SDSQueryExecutionService> logger, ILogService logService, IConfiguration configuration, IHttpContextAccessor context)
        {
            _logger = logger;
            _configuration = configuration;
            _logService = logService;
            _context = context;
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
                    SpineMessageTypeId = (int)GPConnect.Constants.SpineMessageTypes.SpineLdapQuery
                };
                
                var results = new Dictionary<string, object>();
                //ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls13 | SecurityProtocolType.Tls12;

                var useLdaps = bool.Parse(_configuration.GetSection("Spine:sds_use_ldaps").Value);
                var useSdsMutualAuth = bool.Parse(_configuration.GetSection("Spine:sds_use_mutualauth").Value);

                using (var ldapConnection = new LdapConnection
                    {
                        SecureSocketLayer = useLdaps,
                        ConnectionTimeout = int.Parse(_configuration.GetSection("Spine:timeout_seconds").Value) * 1000
                    })
                //LDAP4NET
                //using (var ldapConnection = new LdapConnection
                //{
                //    Timeout = new TimeSpan(0, 0, int.Parse(_configuration.GetSection("Spine:timeout_seconds").Value))
                //})
                {
                    
                    if (useSdsMutualAuth)
                    {
                        _logger.LogInformation($"UseSdsMutualAuth: On");

                        var clientCert = _configuration.GetSection("spine:client_cert").GetConfigurationString();
                        var serverCert = _configuration.GetSection("spine:server_ca_certchain").GetConfigurationString();
                        var clientPrivateKey = _configuration.GetSection("spine:client_private_key").GetConfigurationString();

                        var clientCertData = CertificateHelper.ExtractCertInstances(clientCert);
                        var clientPrivateKeyData = CertificateHelper.ExtractKeyInstance(clientPrivateKey);
                        var x509ClientCertificate = new X509Certificate2(clientCertData.FirstOrDefault());

                        var privateKey = RSA.Create();
                        privateKey.ImportRSAPrivateKey(clientPrivateKeyData, out _);
                        var x509CertificateWithPrivateKey = x509ClientCertificate.CopyWithPrivateKey(privateKey);

                        //LDAP4NET
                        //var pfxFormattedCertificate = new X509Certificate2(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);
                        var pfxFormattedCertificate = new X509Certificate(x509CertificateWithPrivateKey.Export(X509ContentType.Pfx, string.Empty), string.Empty);

                        _clientCertificate = pfxFormattedCertificate;


                        ldapConnection.UserDefinedServerCertValidationDelegate += ValidateServerCertificate;
                        ldapConnection.UserDefinedClientCertSelectionDelegate += SelectLocalCertificate;
                    }

                    var hostName = _configuration.GetSection("Spine:sds_hostname").Value;
                    var hostPort = int.Parse(_configuration.GetSection("Spine:sds_port").Value);

                    _logger.LogInformation("Establishing connection with the LDAP server");
                    _logger.LogInformation($"Host: {hostName}");
                    _logger.LogInformation($"Port: {hostPort}");

                    //ldap4net Library
                    //ldapConnection.Connect(hostName, hostPort, useLdaps ? LdapSchema.LDAPS : LdapSchema.LDAP);

                    ldapConnection.Connect(hostName, hostPort);
                    ldapConnection.Bind(string.Empty, string.Empty);


                    //LDAP4NET
                    //if (_clientCertificate != null)
                    //{

                    //    ldapConnection.SetClientCertificate(_clientCertificate);
                    //    ldapConnection.Bind(LdapAuthType.Anonymous, new LdapCredential());
                    //}

                    //LDAP4NET
                    //ldapConnection.SetOption(LdapOption.LDAP_OPT_PROTOCOL_VERSION, (int)LdapVersion.LDAP_VERSION3);

                    //LDAP4NET
                    //if (useLdaps)
                    //{
                    //    ldapConnection.TrustAllCertificates();
                    //}

                    _logger.LogInformation("Commencing search");
                    _logger.LogInformation($"searchBase is: {searchBase}");
                    _logger.LogInformation($"filter is: {filter}");

                    //LDAP4NET
                    //var searchResults = ldapConnection.Search(searchBase, filter, attributes);

                    var searchResults = ldapConnection.Search(searchBase, LdapConnection.ScopeSub, filter, attributes, false);

                    //LDAP4NET
                    //foreach (var ldapEntry in searchResults)
                    //{
                    //    foreach (var attribute in ldapEntry.ToDirectoryEntry().Attributes)
                    //    {
                    //        results.TryAdd(attribute.Name, attribute.GetValue<string>());
                    //    }
                    //}

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

                    _logger.LogInformation("Search has been executed.");
                }

                var jsonDictionary = JsonConvert.SerializeObject(results);
                if (results.Count > 0)
                {
                    result = JsonConvert.DeserializeObject<T>(jsonDictionary);
                }
                logMessage.ResponsePayload = jsonDictionary;
                logMessage.RoundTripTimeMs = sw.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(logMessage);

                return result;
            }
            catch (LdapException ldapException)
            {
                _logger.LogError($"An LdapException has occurred while attempting to execute an LDAP query", ldapException);
                _logger.LogError($"EXCEPTION: {ldapException}");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError("An Exception has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        private static X509Certificate ValidateClientCertificateSelection(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
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

        private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            if (sslPolicyErrors == SslPolicyErrors.None)
                return true;
            _logger.LogInformation("Certificate error: {0}", sslPolicyErrors);
            return true;
        }


        private static X509Certificate SelectLocalCertificate(object sender, string targetHost, X509CertificateCollection localCertificates, X509Certificate remoteCertificate, string[] acceptableIssuers)
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

        private static bool ValidateServerCertificateChain(X509Certificate2 pfxFormattedCertificate, X509Chain chain, SslPolicyErrors errors)
        {
            if (errors == SslPolicyErrors.None)
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
