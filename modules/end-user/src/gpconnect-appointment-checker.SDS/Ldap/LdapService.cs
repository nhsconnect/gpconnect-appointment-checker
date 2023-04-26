//using gpconnect_appointment_checker.DAL.Interfaces;
//using gpconnect_appointment_checker.DTO.Response.Application;
//using gpconnect_appointment_checker.DTO.Response.Configuration;
//using gpconnect_appointment_checker.Helpers.Enumerations;
//using gpconnect_appointment_checker.SDS.Interfaces;
//using Microsoft.Extensions.Logging;
//using Novell.Directory.Ldap;
//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;

//namespace gpconnect_appointment_checker.SDS
//{
//    public class LdapService : ILdapService
//    {
//        private readonly ILogger<LdapService> _logger;
//        private readonly ILdapRequestExecution _ldapRequestExecution;
//        private readonly IConfigurationService _configurationService;
//        private readonly IApplicationService _applicationService;

//        public LdapService(ILogger<LdapService> logger, ILdapRequestExecution ldapRequestExecution, IConfigurationService configurationService, IApplicationService applicationService)
//        {
//            _logger = logger;
//            _ldapRequestExecution = ldapRequestExecution;
//            _configurationService = configurationService;
//            _applicationService = applicationService;
//        }

//        public List<OrganisationList> GetOrganisationDetailsByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetOrganisationDetailsByOdsCode);
//            try
//            {
//                var processedCodes = new ConcurrentBag<OrganisationList>();
//                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
//                {
//                    var stopwatch = Stopwatch.StartNew();
//                    var processedOrganisation = _ldapRequestExecution.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
//                    processedCodes.Add(new OrganisationList
//                    {
//                        OdsCode = odsCode,
//                        Organisation = processedOrganisation,
//                        ErrorCode = processedOrganisation == null ? errorCodeToRaise : ErrorCode.None,
//                        TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
//                    });
//                    _applicationService.SynchroniseOrganisation(processedOrganisation);
//                });
//                return processedCodes.ToList();
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
//                throw;
//            }
//        }

//        public Organisation GetOrganisationDetailsByOdsCode(string odsCode)
//        {
//            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetOrganisationDetailsByOdsCode);
//            var organisation = _ldapRequestExecution.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
//            _applicationService.SynchroniseOrganisation(organisation);
//            return organisation;
//        }

//        public Spine GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode)
//        {
//            try
//            {
//                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
//                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
//                var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);

//                var spine = response != null ? new Spine
//                {
//                    EndpointAddress = response.EndpointAddress,
//                    AsId = response.AsId,
//                    PartyKey = response.PartyKey
//                } : null;

//                return spine;
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//        }

//        public Spine GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey)
//        {
//            try
//            {
//                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
//                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)).Replace("{partyKey}", Regex.Escape(partyKey));
//                var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);

//                var spine = response != null ? new Spine
//                {
//                    AsId = response.AsId,
//                    PartyKey = response.PartyKey,
//                    ProductName = response.ProductName
//                } : null;

//                return spine;
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//        }

//        public Spine GetGpConsumerAsIdByOdsCode(string odsCode)
//        {
//            try
//            {
//                var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpConsumerAsIdByOdsCode);
//                var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
//                var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);
//                var spine = response != null ? new Spine
//                {
//                    EndpointAddress = response.EndpointAddress,
//                    AsId = response.AsId,
//                    PartyKey = response.PartyKey
//                } : null;

//                return spine;
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//        }

//        public List<SpineList> GetGpProviderEndpointAndPartyKeyByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
//            try
//            {
//                var processedCodes = new ConcurrentBag<SpineList>();
//                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
//                {
//                    var stopwatch = Stopwatch.StartNew();
//                    var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
//                    processedCodes.Add(new SpineList
//                    {
//                        OdsCode = odsCode,
//                        PartyKey = response?.PartyKey,
//                        Spine = response != null ? new Spine()
//                        {
//                            EndpointAddress = response.EndpointAddress,
//                            AsId = response.AsId,
//                            PartyKey = response.PartyKey
//                        } : null,
//                        ErrorCode = response == null ? errorCodeToRaise : ErrorCode.None,
//                        TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
//                    });
//                });
//                return processedCodes.ToList();
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
//                throw;
//            }
//        }

//        public List<SpineList> GetGpConsumerAsIdByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
//        {
//            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpConsumerAsIdByOdsCode);
//            try
//            {
//                var processedCodes = new ConcurrentBag<SpineList>();
//                Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, (odsCode) =>
//                {
//                    var stopwatch = Stopwatch.StartNew();
//                    var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
//                    processedCodes.Add(new SpineList
//                    {
//                        OdsCode = odsCode,
//                        Spine = response != null ? new Spine
//                        {
//                            EndpointAddress = response.EndpointAddress,
//                            AsId = response.AsId,
//                            PartyKey = response.PartyKey
//                        } : null,
//                        ErrorCode = response == null ? errorCodeToRaise : ErrorCode.None,
//                        TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
//                    });
//                });
//                return processedCodes.ToList();
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
//                throw;
//            }
//        }

//        public List<SpineList> GetGpProviderAsIdByOdsCodeAndPartyKey(List<SpineList> odsCodesWithPartyKeys)
//        {
//            var sdsQuery = GetSdsQueryByName(Constants.LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
//            try
//            {
//                odsCodesWithPartyKeys.AsParallel().WithDegreeOfParallelism(Environment.ProcessorCount).WithExecutionMode(ParallelExecutionMode.ForceParallelism)
//                    .Where(x => !string.IsNullOrEmpty(x.PartyKey)).ForAll(odsCodeWithPartyKey =>
//                {
//                    var stopwatch = Stopwatch.StartNew();
//                    var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCodeWithPartyKey.OdsCode)).Replace("{partyKey}", Regex.Escape(odsCodeWithPartyKey.PartyKey)), sdsQuery.QueryAttributesAsArray);

//                    odsCodeWithPartyKey.Spine.AsId = response?.AsId;
//                    odsCodeWithPartyKey.Spine.ProductName = response?.ProductName;
//                    //odsCodeWithPartyKey.Spine.EndpointAddress = response?.EndpointAddress;
//                    odsCodeWithPartyKey.Spine.PartyKey = response?.PartyKey;
//                    odsCodeWithPartyKey.ErrorCode = response?.AsId == null
//                        ? ErrorCode.ProviderASIDCodeNotFound
//                        : ErrorCode.None;
//                    odsCodeWithPartyKey.TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds + odsCodeWithPartyKey.TimeTakenInSeconds;
//                });
//                return odsCodesWithPartyKeys;
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, $"An error has occurred while attempting to execute an LDAP query - sdsQuery is {sdsQuery.QueryText} - searchBase is {sdsQuery.SearchBase}");
//                throw;
//            }
//        }

//        private SdsQuery GetSdsQueryByName(string queryName)
//        {
//            try
//            {
//                var sdsQuery = _configurationService.GetSdsQueryConfiguration(queryName);
//                return sdsQuery;
//            }
//            catch (LdapException ldapException)
//            {
//                _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
//                throw;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, "An error has occurred while attempting to load LDAP queries from the database");
//                throw;
//            }
//        }
//    }
//}
