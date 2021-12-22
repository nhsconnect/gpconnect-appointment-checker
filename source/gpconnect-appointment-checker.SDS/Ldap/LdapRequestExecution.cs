using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Security;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapRequestExecution : SdsQueryExecutionBase, ILdapRequestExecution
    {
        private static ILogger<LdapRequestExecution> _logger;
        private readonly ILogService _logService;
        private static bool _haveLoggedTlsVersion = false;

        public LdapRequestExecution(ILogger<LdapRequestExecution> logger, ILogService logService, IOptionsMonitor<Spine> spineOptionsDelegate) : base(logger, null, null, spineOptionsDelegate)
        {
            _logger = logger;
            _logService = logService;
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

                var ldapConnectionOptions = new LdapConnectionOptions();

                if (_spineOptionsDelegate.CurrentValue.SdsUseLdaps)
                {
                    ldapConnectionOptions.ConfigureSslProtocols(SecurityHelper.ParseTlsVersion(_spineOptionsDelegate.CurrentValue.SdsTlsVersion));
                    ldapConnectionOptions.UseSsl();
                    ldapConnectionOptions.ConfigureLocalCertificateSelectionCallback(SelectLocalCertificate);
                    ldapConnectionOptions.ConfigureRemoteCertificateValidationCallback(ValidateServerCertificate);
                }

                using (var ldapConnection = new LdapConnection(ldapConnectionOptions)
                {
                    ConnectionTimeout = _spineOptionsDelegate.CurrentValue.TimeoutMilliseconds
                })
                {
                    SetupMutualAuth();
                    
                    ldapConnection.Connect(_spineOptionsDelegate.CurrentValue.SdsHostname, _spineOptionsDelegate.CurrentValue.SdsPort);
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
                logMessage.RoundTripTimeMs = sw.ElapsedMilliseconds;
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
                string tlsVersion = GetTlsVersionInUse(ldapConnection);
                _logger.LogInformation($"LDAP TLS version in use: {tlsVersion}");
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
    }
}
