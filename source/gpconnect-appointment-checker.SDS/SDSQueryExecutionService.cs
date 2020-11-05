using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace gpconnect_appointment_checker.SDS
{
    public class SDSQueryExecutionService : ISDSQueryExecutionService
    {
        private readonly ILogger<SDSQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;
        private readonly LdapConnection _connection;
        private readonly IHttpContextAccessor _context;

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

        private ILdapConnection GetConnection()
        {
            try
            {
                var ldapConn = _connection;
                var hostName = _configuration.GetSection("Spine:sds_hostname").Value;
                var hostPort = int.Parse(_configuration.GetSection("Spine:sds_port").Value);
                if (ldapConn == null && !string.IsNullOrEmpty(hostName) && hostPort > 0)
                {
                    ldapConn = new LdapConnection
                    {
                        SecureSocketLayer = bool.Parse(_configuration.GetSection("Spine:sds_use_ldaps").Value),
                        ConnectionTimeout = int.Parse(_configuration.GetSection("Spine:timeout_seconds").Value) * 1000
                    };
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
    }
}
