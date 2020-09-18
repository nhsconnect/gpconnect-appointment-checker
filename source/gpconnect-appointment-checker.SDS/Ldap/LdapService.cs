using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Novell.Directory.Ldap;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        private readonly IConfigurationService _configurationService;
        private readonly LdapConnection _connection;

        public LdapService(ILogger<LdapService> logger, IConfigurationService configurationService)
        {
            _logger = logger;
            _configurationService = configurationService;
        }

        public async Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=organisations, o=nhs";
                var filter = $"(uniqueidentifier={odsCode})";
                var attributes = new [] {"postalAddress", "postalCode", "nhsIDCode", "nhsOrgType", "o"};

                var results = await ExecuteLdapQuery<Organisation>(searchBase, filter, null);
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Organisation> OrganisationHasAppointmentsProviderSystemByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filter = $"(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";
                var attributes = new[] { "nhsMhsSvcIA", "nhsMHSPartyKey", "nhsIDCode", "nhsMHSEndPoint" };

                var results = await ExecuteLdapQuery<Organisation>(searchBase, filter, attributes);
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Organisation> OrganisationHasAppointmentsConsumerSystemByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filter = $"(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";
                var attributes = new[] { "nhsAsSvcIA", "nhsMhsPartyKey", "uniqueIdentifier", "nhsIDCode" };

                var results = await ExecuteLdapQuery<Organisation>(searchBase, filter, attributes);
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        public async Task<Spine> GetGpProviderEndpointAndAsIdByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = "ou=services, o=nhs";
                var filterToGetEndpointAndPartyKey = $"(&(nhsIDCode={odsCode})(objectClass=nhsMhs)(nhsMhsSvcIA=urn:nhs:names:services:gpconnect:structured:fhir:rest:read:metadata-1))";
                var attributesToGetEndpointAndPartyKey = new[] { "nhsMhsEndPoint", "nhsMhsPartyKey" };
                var resultEndpointAndPartyKey = await ExecuteLdapQuery<Spine>(searchBase, filterToGetEndpointAndPartyKey, attributesToGetEndpointAndPartyKey);
                
                var partyKey = resultEndpointAndPartyKey.Party_Key;
                var filterToGetAsId = $"(&(nhsIDCode={odsCode})(objectClass=nhsAs)(nhsMhsPartyKey={partyKey}))";
                var attributesToGetAsId = new[] { "uniqueidentifier" };
                var results = await ExecuteLdapQuery<Spine>(searchBase, filterToGetAsId, attributesToGetAsId);
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        private async Task<T> ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class
        {
            try
            {
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
