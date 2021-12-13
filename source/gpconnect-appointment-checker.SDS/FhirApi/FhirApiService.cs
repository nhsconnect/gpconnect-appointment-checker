using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using gpconnect_appointment_checker.SDS.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Organisation = gpconnect_appointment_checker.DTO.Response.Application.Organisation;
using Spine = gpconnect_appointment_checker.DTO.Response.Configuration.Spine;

namespace gpconnect_appointment_checker.SDS
{
    public class FhirApiService : IFhirApiService
    {
        private readonly ILogger<FhirApiService> _logger;
        private readonly IFhirRequestExecution _fhirRequestExecution;
        private readonly IConfigurationService _configurationService;
        private readonly IApplicationService _applicationService;
        private readonly IOptionsMonitor<Spine> _spineOptionsDelegate;

        public FhirApiService(ILogger<FhirApiService> logger, IFhirRequestExecution fhirRequestExecution, IConfigurationService configurationService, IApplicationService applicationService, IOptionsMonitor<Spine> spineOptionsDelegate)
        {
            _logger = logger;
            _fhirRequestExecution = fhirRequestExecution;
            _configurationService = configurationService;
            _applicationService = applicationService;
            _spineOptionsDelegate = spineOptionsDelegate;
        }

        public async Task<List<OrganisationList>> GetOrganisationDetailsByOdsCode(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetOrganisationDetailsByOdsCode.ToString());
            var tasks = new ConcurrentBag<Task<OrganisationList>>();
            Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, async (odsCode) =>
            {
                var stopwatch = Stopwatch.StartNew();
                var organisation = await GetAndMapOrganisationResponse(odsCode, fhirApiQuery);
                tasks.Add(Task.FromResult(new OrganisationList
                {
                    OdsCode = odsCode,
                    Organisation = organisation,
                    ErrorCode = organisation == null ? errorCodeToRaise : ErrorCode.None,
                    TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
                }));
                _applicationService.SynchroniseOrganisation(organisation);
            });

            var processedCodes = await Task.WhenAll(tasks);
            return processedCodes.OrderBy(o => odsCodes.IndexOf(o.OdsCode)).ToList();
        }

        public async Task<Organisation> GetOrganisationDetailsByOdsCode(string odsCode)
        {
            if (!string.IsNullOrEmpty(odsCode))
            {
                var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetOrganisationDetailsByOdsCode.ToString());
                var organisation = await GetAndMapOrganisationResponse(odsCode, fhirApiQuery);
                _applicationService.SynchroniseOrganisation(organisation);
                return organisation;
            }
            return null;
        }

        public async Task<Spine> GetProviderDetails(string odsCode)
        {
            var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetRoutingReliabilityDetailsFromSDS.ToString());
            var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) } });
            
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            var spineProviderDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiSystemsRegisterFqdn, token);
            
            if (spineProviderDetails != null && spineProviderDetails.Entries != null && spineProviderDetails.Entries.Count > 0)
            {
                var spineProviderAsId = await GetGpProviderAsIdByOdsCodeAndPartyKey(odsCode, spineProviderDetails.PartyKey);

                var spine = new Spine
                {
                    PartyKey = spineProviderDetails.PartyKey,
                    AsId = spineProviderAsId?.AsId,
                    OdsCode = odsCode,
                    ManufacturingOrganisationOdsCode = spineProviderAsId?.ManufacturingOrganisationOdsCode,
                    EndpointAddress = spineProviderDetails.EndpointAddress,
                    ProductName = await GetProductName(spineProviderAsId?.ManufacturingOrganisationOdsCode)
                };                
                return spine;
            }
            return null;
        }

        private async Task<string> GetProductName(string manufacturingOrganisationOdsCode)
        {
            var publisherOrganisation = await GetOrganisationDetailsByOdsCode(manufacturingOrganisationOdsCode);
            return publisherOrganisation?.OrganisationName;
        }

        public async Task<Spine> GetConsumerDetails(string odsCode)
        {
            if (!string.IsNullOrEmpty(odsCode))
            {
                var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetAccreditedSystemDetailsForConsumerFromSDS.ToString());
                var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) }});
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                var spineConsumerDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiSystemsRegisterFqdn, token);

                var spine = new Spine
                {
                    AsId = spineConsumerDetails?.AsId,
                    OdsCode = odsCode
                };
                return spine;
            }
            return null;
        }

        public async Task<List<SpineList>> GetProviderDetails(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetRoutingReliabilityDetailsFromSDS.ToString());
            var tasks = new ConcurrentBag<Task<SpineList>>();
            Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, async (odsCode) =>
            {
                var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) } });
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                var stopwatch = Stopwatch.StartNew();
                var spineProviderDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiSystemsRegisterFqdn, token);
                
                if (spineProviderDetails != null && spineProviderDetails.PartyKey != null)
                {
                    var spineProviderAsId = await GetGpProviderAsIdByOdsCodeAndPartyKey(odsCode, spineProviderDetails.PartyKey);
                    
                    tasks.Add(Task.FromResult(new SpineList
                    {
                        OdsCode = odsCode,
                        PartyKey = spineProviderDetails.PartyKey,
                        Spine = new Spine
                        {
                            PartyKey = spineProviderDetails.PartyKey,
                            AsId = spineProviderAsId?.AsId,
                            OdsCode = odsCode,
                            ManufacturingOrganisationOdsCode = spineProviderAsId?.ManufacturingOrganisationOdsCode,
                            EndpointAddress = spineProviderDetails.EndpointAddress,
                            ProductName = await GetProductName(spineProviderAsId?.ManufacturingOrganisationOdsCode)
                        },
                        ErrorCode = spineProviderDetails == null ? errorCodeToRaise : ErrorCode.None,
                        TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
                    }));
                }
                else
                {
                    tasks.Add(Task.FromResult(new SpineList
                    {
                        OdsCode = odsCode,
                        ErrorCode = errorCodeToRaise,
                        TimeTakenInSeconds = stopwatch.Elapsed.TotalSeconds
                    }));
                }
            });
            var processedCodes = await Task.WhenAll(tasks);
            return processedCodes.OrderBy(o => odsCodes.IndexOf(o.OdsCode)).ToList();
        }

        private async Task<DTO.Response.Fhir.Spine> GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey)
        {
            var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetAccreditedSystemDetailsFromSDS.ToString());
            var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) },
                    { "{partyKey}", Regex.Escape(partyKey) }});
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var result = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiSystemsRegisterFqdn, token);
            return result;
        }

        public async Task<List<SpineList>> GetConsumerDetails(List<string> odsCodes, ErrorCode errorCodeToRaise)
        {
            var fhirApiQuery = _configurationService.GetFhirApiQueryConfiguration(Enumerations.FhirQueryTypes.GetAccreditedSystemDetailsForConsumerFromSDS.ToString());
            var tasks = new ConcurrentBag<Task<SpineList>>();

            Parallel.ForEach(odsCodes, new ParallelOptions { MaxDegreeOfParallelism = Convert.ToInt32(Math.Ceiling((Environment.ProcessorCount * 0.75) * 2.0)) }, async (odsCode) =>
            {
                var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) }});
                var tokenSource = new CancellationTokenSource();
                var token = tokenSource.Token;
                var stopwatch = Stopwatch.StartNew();
                var spineConsumerDetails = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Spine>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiSystemsRegisterFqdn, token);
                tasks.Add(Task.FromResult(new SpineList
                {
                    OdsCode = odsCode,
                    Spine = spineConsumerDetails != null ? new Spine
                    {
                        EndpointAddress = spineConsumerDetails.EndpointAddress,
                        AsId = spineConsumerDetails.AsId,
                        PartyKey = spineConsumerDetails.PartyKey
                    } : null,
                    ErrorCode = spineConsumerDetails == null ? errorCodeToRaise : ErrorCode.None,
                    TimeTakenInSeconds = stopwatch.Elapsed.Seconds
                }));
            });
            var processedCodes = await Task.WhenAll(tasks);
            return processedCodes.OrderBy(o => odsCodes.IndexOf(o.OdsCode)).ToList();
        }

        private async Task<Organisation> GetAndMapOrganisationResponse(string odsCode, FhirApiQuery fhirApiQuery)
        {
            var query = fhirApiQuery.QueryText.SearchAndReplace(new Dictionary<string, string> { { "{odsCode}", Regex.Escape(odsCode) } });
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;
            var response = await _fhirRequestExecution.ExecuteFhirQuery<DTO.Response.Fhir.Organisation>(query, _spineOptionsDelegate.CurrentValue.SpineFhirApiDirectoryServicesFqdn, token, SpineMessageTypes.SpineFhirApiOrganisationQuery);

            if (response != null && !response.HasErrored && response.Issue == null)
            {
                var organisation = new Organisation
                {
                    OdsCode = odsCode,
                    OrganisationName = response.OrganisationName,
                    OrganisationTypeCode = response.Type.Coding.OrganisationTypeDisplay,
                    PostalAddress = response.PostalAddress.PostalAddressCommaSeparated,
                    PostalCode = response.PostalAddress.PostalCode
                };
                return organisation;
            }
            return null;
        }
    }
}
