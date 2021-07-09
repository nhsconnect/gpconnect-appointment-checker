using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.Helpers.Enumerations;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Novell.Directory.Ldap;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS
{
    public class LdapService : ILdapService
    {
        private readonly ILogger<LdapService> _logger;
        private readonly ISDSQueryExecutionService _sdsQueryExecutionService;
        private readonly IConfigurationService _configurationService;
        private readonly IApplicationService _applicationService;

        public LdapService(ILogger<LdapService> logger, ISDSQueryExecutionService sdsQueryExecutionService, IConfigurationService configurationService, IApplicationService applicationService)
        {
            _logger = logger;
            _sdsQueryExecutionService = sdsQueryExecutionService;
            _configurationService = configurationService;
            _applicationService = applicationService;
        }

        public List<OrganisationList> GetOrganisationDetailsByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetOrganisationDetailsByOdsCode);
            try
            {
                var processedCodes = new ConcurrentBag<OrganisationList>();
                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
                {
                    var processedOrganisation = _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
                    processedCodes.Add(new OrganisationList
                    {
                        OdsCode = odsCode,
                        Organisation = processedOrganisation,
                        ErrorCode = processedOrganisation == null ? errorCodeToRaise : ErrorCode.None
                    });
                    _applicationService.SynchroniseOrganisation(processedOrganisation);
                });
                return processedCodes.ToList();
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
                throw;
            }
        }

        public Organisation GetOrganisationDetailsByOdsCode(string odsCode)
        {
            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetOrganisationDetailsByOdsCode);
            var organisation = _sdsQueryExecutionService.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
            _applicationService.SynchroniseOrganisation(organisation);
            return organisation;
        }

        public Spine GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode)
        {
            try
            {
                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
                var result = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);
                return result;
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
                throw;
            }
        }

        public Spine GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey)
        {
            try
            {
                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)).Replace("{partyKey}", Regex.Escape(partyKey));
                var result = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);
                return result;
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
                throw;
            }
        }

        public Spine GetGpConsumerAsIdByOdsCode(string odsCode)
        {
            try
            {
                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpConsumerAsIdByOdsCode);
                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
                var result = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);
                return result;
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
                throw;
            }
        }

        public List<SpineList> GetGpProviderEndpointAndPartyKeyByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
            try
            {
                var processedCodes = new ConcurrentBag<SpineList>();
                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
                {
                    var processedOrganisation = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
                    processedCodes.Add(new SpineList
                    {
                        OdsCode = odsCode,
                        PartyKey = processedOrganisation?.party_key,
                        Spine = processedOrganisation,
                        ErrorCode = processedOrganisation == null ? errorCodeToRaise : ErrorCode.None
                    });
                });
                return processedCodes.ToList();
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
                throw;
            }
        }

        public List<SpineList> GetGpConsumerAsIdByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpConsumerAsIdByOdsCode);
            try
            {
                var processedCodes = new ConcurrentBag<SpineList>();
                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
                {
                    var result = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
                    processedCodes.Add(new SpineList
                    {
                        OdsCode = odsCode,
                        ErrorCode = result == null ? errorCodeToRaise : ErrorCode.None
                    });
                });
                return processedCodes.ToList();
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
                throw;
            }
        }

        public List<SpineList> GetGpProviderAsIdByOdsCodeAndPartyKey(List<SpineList> odsCodesWithPartyKeys)
        {
            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
            try
            {
                odsCodesWithPartyKeys.AsParallel().WithDegreeOfParallelism(Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)))
                    .Where(x => !string.IsNullOrEmpty(x.PartyKey)).ForAll(odsCodeWithPartyKey =>
                //Parallel.ForEach(odsCodesWithPartyKeys.Where(x => !string.IsNullOrEmpty(x.PartyKey)), new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCodeWithPartyKey) =>
                {
                    var processedOrganisation = _sdsQueryExecutionService.ExecuteLdapQuery<Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCodeWithPartyKey.OdsCode)).Replace("{partyKey}", Regex.Escape(odsCodeWithPartyKey.PartyKey)), sdsQuery.QueryAttributesAsArray);
                    odsCodeWithPartyKey.Spine.asid = processedOrganisation?.asid;
                    odsCodeWithPartyKey.Spine.product_name = processedOrganisation?.product_name;
                    odsCodeWithPartyKey.ErrorCode = processedOrganisation?.asid == null
                        ? ErrorCode.ProviderASIDCodeNotFound
                        : ErrorCode.None;
                });
                return odsCodesWithPartyKeys;
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
                throw;
            }
        }

        private SdsQuery GetSdsQueryByName(string queryName)
        {
            try
            {
                var sdsQueryList = _configurationService.GetSdsQueryConfiguration();
                return sdsQueryList.FirstOrDefault(x => x.QueryName == queryName);
            }
            catch (LdapException ldapException)
            {
                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred while attempting to load LDAP queries from the database");
                throw;
            }
        }
    }
}
