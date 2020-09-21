using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DAL.Logging;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class QueryExecutionService : IQueryExecutionService
    {
        private readonly ILogger<QueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;
        private readonly LdapConnection _connection;

        public QueryExecutionService(ILogger<QueryExecutionService> logger, IConfigurationService configurationService, ILogService logService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
        }

        public async Task<T> ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class
        {
            try
            {
                var sw = new Stopwatch();
                sw.Start();
                var logMessage = new SpineMessage
                {
                    RequestPayload = $"{searchBase} {filter} {attributes}",
                    SpineMessageTypeId = (int)LogService.SpineMessageTypes.SDSLdapQuery
                };
                var ldapConnection = await GetConnection();
                var results = new Dictionary<string, object>();
                var searchResults = ldapConnection.Search(searchBase, LdapConnection.ScopeSub, filter, attributes, false);

                while (searchResults.HasMore())
                {
                    var nextEntry = searchResults.Next();
                    var attributeSet = nextEntry.GetAttributeSet();

                    foreach (var attribute in attributeSet)
                    {
                        results.Add(attribute.Name, attribute.StringValue);
                    }
                }

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
                throw;
            }
        }

        private async Task<ILdapConnection> GetConnection()
        {
            try
            {
                var spineConnectionSettings = await _configurationService.GetSpineConfiguration();
                var ldapConn = _connection;

                if (ldapConn == null)
                {
                    string userName = string.Empty;
                    string password = string.Empty;

                    ldapConn = new LdapConnection { SecureSocketLayer = spineConnectionSettings.SDS_Use_Ldaps };
                    ldapConn.Connect(spineConnectionSettings.SDS_Hostname, spineConnectionSettings.SDS_Port);
                    ldapConn.Bind(userName, password);
                }
                return ldapConn;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to establish a connection to the LDAP server", exc);
                throw;
            }
        }
    }
}
