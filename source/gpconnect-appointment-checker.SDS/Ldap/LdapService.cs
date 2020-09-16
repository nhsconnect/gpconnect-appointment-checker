using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        private readonly IConfiguration _configuration;
        private static readonly ILdapConnection _connection;
        private readonly IConfigurationService _configurationService;

        public LdapService(IConfiguration configuration, ILogger<LdapService> logger, IConfigurationService configurationService)
        {
            _logger = logger;
            _configuration = configuration;
            _configurationService = configurationService;
        }

        public async Task<Dictionary<string, string>> GetOrganisationDetailsByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=organisations, o=nhs";
                var filter = $"(uniqueidentifier={odsCode})";

                var results = await ExecuteLdapQuery(searchBase, filter, "");
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Dictionary<string, string>> GetGpProviderEndpointAndASIDByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                //var filter = _configuration[$"LdapQueries:{LdapQueries.GetGpProviderEndpointAndASIDByOdsCode}"].Replace("[odsCode]", odsCode);
                var filter = $"(&(nhsIDCode={odsCode}) (objectClass=nhsMhs) (nhsMhsSvcIA=urn:nhs:names:services:gpconnect:fhir:operation:gpc.getstructuredrecord-1))";
                var results = await ExecuteLdapQuery(searchBase, filter, "");
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Dictionary<string, string>> OrganisationHasAppointmentsProviderSystemByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filter = $"(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";

                var results = await ExecuteLdapQuery(searchBase, filter, "");
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Dictionary<string, string>> OrganisationHasAppointmentsConsumerSystemByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filter = $"(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";

                var results = await ExecuteLdapQuery(searchBase, filter, "");
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Dictionary<string, string>> GetGpProviderEndpointAndAsIdByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filterToGetEndpointAndPartyKey = $"(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";
                var resultEndpointAndPartyKey = await ExecuteLdapQuery(searchBase, filterToGetEndpointAndPartyKey, "");
                var partyKey = resultEndpointAndPartyKey.Where(x => x.Key == "nhsMHSPartyKey");
                
                var filterToGetAsId = $"(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsMhsPartyKey={partyKey}))";
                var results = await ExecuteLdapQuery(searchBase, filterToGetAsId, "");
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        private async Task<Dictionary<string, string>> ExecuteLdapQuery(string searchBase, string filter, string attributeName)
        {
            try
            {
                var ldapConnection = await GetConnection();
                var results = new Dictionary<string, string>();
                var searchResults = ldapConnection.Search(searchBase, LdapConnection.ScopeSub, filter, new string[] { "postalAddress", "postalCode", "nhsIDCode", "nhsOrgType" }, false);

                while (searchResults.HasMore())
                {
                    var nextEntry = searchResults.Next();
                    nextEntry.GetAttributeSet();
                    var attr = nextEntry.GetAttribute(attributeName);
                    if (attr != null)
                    {
                        results.Add(attributeName, nextEntry.GetAttribute(attributeName).StringValue);
                    }
                }
                return results;
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
                var ldapConn = _connection as LdapConnection;

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
