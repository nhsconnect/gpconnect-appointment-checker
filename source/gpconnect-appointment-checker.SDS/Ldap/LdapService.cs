using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        private readonly ISDSQueryExecutionService _sdsQueryExecutionService;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationService _configurationService;
        private readonly List<SdsQuery> _sdsQueries;

        public LdapService(ILogger<LdapService> logger, ISDSQueryExecutionService sdsQueryExecutionService, IConfiguration configuration, IConfigurationService configurationService)
        {
            _logger = logger;
            _sdsQueryExecutionService = sdsQueryExecutionService;
            _configuration = configuration;
            _configurationService = configurationService;
        }

        public async Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode)
        {
            try
            {
                var sdsQuery = await GetSdsQueryByName(Constants.LdapQuery.GetOrganisationDetailsByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", odsCode);
                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, filter);
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
                var sdsQuery = await GetSdsQueryByName(Constants.LdapQuery.OrganisationHasAppointmentsProviderSystemByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", odsCode);
                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, filter);
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
                var sdsQuery = await GetSdsQueryByName(Constants.LdapQuery.OrganisationHasAppointmentsConsumerSystemByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", odsCode);
                var results = await _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, filter);
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
                var sdsQuery = await GetSdsQueryByName(Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", odsCode);
                var result = await _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, filter);

                if (result != null)
                {
                    sdsQuery = await GetSdsQueryByName(Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
                    filter = sdsQuery.QueryText.Replace("{odsCode}", odsCode).Replace("{partyKey}", result.PartyKey);
                    var result2 = await _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, filter);
                    result.AsId = result2.AsId;
                }
                return result;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to execute an LDAP query", exc);
                throw;
            }
        }

        private async Task<SdsQuery> GetSdsQueryByName(string queryName)
        {
            try
            {
                var sdsQueryList = _sdsQueries ?? await _configurationService.GetSdsQueryConfiguration();
                return sdsQueryList.FirstOrDefault(x => x.QueryName == queryName);
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred while attempting to load LDAP queries from the database", exc);
                throw;
            }
        }
    }
}
