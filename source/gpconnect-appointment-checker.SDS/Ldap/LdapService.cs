using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        private readonly ISDSQueryExecutionService _sdsQueryExecutionService;
        private readonly IConfiguration _configuration;

        public LdapService(ILogger<LdapService> logger, ISDSQueryExecutionService sdsQueryExecutionService, IConfiguration configuration)
        {
            _logger = logger;
            _sdsQueryExecutionService = sdsQueryExecutionService;
            _configuration = configuration;
        }

        public async Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode)
        {
            try
            {
                var searchBase = _configuration[Constants.SearchBase.Organisation];
                var filter = _configuration[Constants.LdapQuery.GetOrganisationDetailsByOdsCode].Replace("{odsCode}", odsCode);
                var attributes = new [] {"postalAddress", "postalCode", "nhsIDCode", "nhsOrgType", "o"};

                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(searchBase, filter, null);
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
                var searchBase = _configuration[Constants.SearchBase.Services];
                var filter = _configuration[Constants.LdapQuery.OrganisationHasAppointmentsProviderSystemByOdsCode].Replace("{odsCode}", odsCode);
                var attributes = new[] { "nhsMhsSvcIA", "nhsMHSPartyKey", "nhsIDCode", "nhsMHSEndPoint" };

                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(searchBase, filter, attributes);
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
                var searchBase = _configuration[Constants.SearchBase.Services];
                var filter = _configuration[Constants.LdapQuery.OrganisationHasAppointmentsConsumerSystemByOdsCode].Replace("{odsCode}", odsCode);
                var attributes = new[] { "nhsAsSvcIA", "nhsMhsPartyKey", "uniqueIdentifier", "nhsIDCode" };

                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(searchBase, filter, attributes);
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
                var searchBase = _configuration[Constants.SearchBase.Services];
                var filterToGetEndpointAndPartyKey = _configuration[Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode].Replace("{odsCode}", odsCode);
                var attributesToGetEndpointAndPartyKey = new[] { "nhsMhsEndPoint", "nhsMhsPartyKey" };
                var result = await _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(searchBase, filterToGetEndpointAndPartyKey, attributesToGetEndpointAndPartyKey);

                if (result != null)
                {
                    var filterToGetAsId = _configuration[Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey]
                        .Replace("{odsCode}", odsCode).Replace("{partyKey}", result.Party_Key);

                    var attributesToGetAsId = new[] {"uniqueidentifier"};
                    var asIdResult = await _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(searchBase, filterToGetAsId, attributesToGetAsId);
                    result.AsId = asIdResult.AsId;
                }
                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }
    }
}
