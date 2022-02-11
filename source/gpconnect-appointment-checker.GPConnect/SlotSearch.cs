using gpconnect_appointment_checker.DTO;
using gpconnect_appointment_checker.DTO.Request.Audit;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        private async Task<SlotSimple> GetFreeSlots(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress)
        {
            var getRequest = new HttpRequestMessage();

            try
            {
                var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                    x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectSearchFreeSlots);
                requestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectSearchFreeSlots;
                requestParameters.InteractionId = spineMessageType?.InteractionId;

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                _spineMessage.SpineMessageTypeId = requestParameters.SpineMessageTypeId;

                var client = _httpClientFactory.CreateClient("GpConnectClient");

                client.Timeout = new TimeSpan(0, 0, 30);
                AddRequiredRequestHeaders(requestParameters, client);
                _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                var requestUri = new Uri($"{requestParameters.EndpointAddressWithSpineSecureProxy}/Slot");
                var uriBuilder = AddQueryParameters(requestParameters, startDate, endDate, requestUri);

                getRequest.Method = HttpMethod.Get;
                getRequest.RequestUri = uriBuilder.Uri;

                var response = await client.SendAsync(getRequest);

                var responseStream = await response.Content.ReadAsStringAsync();

                _spineMessage.ResponsePayload = responseStream;
                _spineMessage.ResponseStatus = response.StatusCode.ToString();
                _spineMessage.RequestPayload = getRequest.ToString();
                _spineMessage.ResponseHeaders = response.Headers.ToString();

                stopWatch.Stop();
                _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(_spineMessage);

                var slotSimple = new SlotSimple()
                {
                    CurrentSlotEntrySimple = new List<SlotEntrySimple>(),
                    PastSlotEntrySimple = new List<SlotEntrySimple>(),
                    Issue = new List<Issue>()
                };

                var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

                if (results.Issue?.Count > 0)
                {
                    slotSimple.Issue = results.Issue;
                    return slotSimple;
                }

                var slotResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Slot).ToList();
                if (slotResources == null || slotResources?.Count == 0) return slotSimple;

                var practitionerResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Practitioner).ToList();
                var locationResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Location).ToList();
                var scheduleResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Schedule).ToList();

                var slotList = (from slot in slotResources?.Where(s => s.resource != null)
                                let practitioner = GetPractitionerDetails(slot.resource.schedule.reference, scheduleResources, practitionerResources)
                                let location = GetLocation(slot.resource.schedule.reference, scheduleResources, locationResources)
                                let schedule = GetSchedule(slot.resource.schedule.reference, scheduleResources)
                                select new SlotEntrySimple
                                {
                                    AppointmentDate = slot.resource.start.GetValueOrDefault().DateTime,
                                    SessionName = schedule.resource.serviceCategory?.text,
                                    StartTime = slot.resource.start.GetValueOrDefault().DateTime,
                                    Duration = slot.resource.start.DurationBetweenTwoDates(slot.resource.end),
                                    SlotType = slot.resource.serviceType.FirstOrDefault()?.text,
                                    DeliveryChannel = slot.resource.extension?.FirstOrDefault()?.valueCode,
                                    PractitionerGivenName = practitioner?.name?.FirstOrDefault()?.given?.FirstOrDefault(),
                                    PractitionerFamilyName = practitioner?.name?.FirstOrDefault()?.family,
                                    PractitionerPrefix = practitioner?.name?.FirstOrDefault()?.prefix?.FirstOrDefault(),
                                    PractitionerRole = schedule.resource.extension?.FirstOrDefault()?.valueCodeableConcept?.coding?.FirstOrDefault()?.display,
                                    PractitionerGender = practitioner?.gender,
                                    LocationName = location?.name,
                                    LocationAddressLines = location?.address?.line,
                                    LocationAddressLinesAsString = AddressBuilder.GetFullAddress(location?.address?.line, location?.address?.district, location?.address?.city, location?.address?.postalCode, location?.address?.country),
                                    LocationCity = location?.address?.city,
                                    LocationCountry = location?.address?.country,
                                    LocationDistrict = location?.address?.district,
                                    LocationPostalCode = location?.address?.postalCode,
                                    SlotInPast = slot.resource.start.GetValueOrDefault().DateTime <= _currentDateTime
                                }).OrderBy(z => z.LocationName)
                    .ThenBy(s => s.AppointmentDate)
                    .ThenBy(s => s.StartTime);

                slotSimple.CurrentSlotEntrySimple.AddRange(slotList.Where(x => !x.SlotInPast));
                slotSimple.PastSlotEntrySimple.AddRange(slotList.Where(x => x.SlotInPast));

                return slotSimple;
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, "A timeout error has occurred");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error occurred in trying to execute a GET request - {getRequest}");
                throw;
            }
        }

        private async Task<List<SlotEntrySummaryCount>> GetFreeSlotsSummary(List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetails, List<RequestParametersList> requestParameterList, DateTime startDate, DateTime endDate, CancellationToken cancellationToken, SearchType searchType)
        {
            try
            {
                var emptyRequestParameters = requestParameterList.Where(x => x.RequestParameters == null)
                    .Select(requestParameter => new SlotEntrySummaryCount
                    {
                        OdsCode = requestParameter.OdsCode,
                        SpineMessageId = null
                    }).ToArray();

                var client = _httpClientFactory.CreateClient("GpConnectClient");
                var semaphore = new SemaphoreSlim(requestParameterList.Count, requestParameterList.Count);

                var tasks = requestParameterList.Where(x => x.RequestParameters != null && x.RequestParameters.EndpointAddress != null)
                    .Select(requestParameter => ProcessResults(organisationErrorCodeOrDetails, searchType, startDate, endDate, requestParameter, semaphore, client, cancellationToken));                    

                var processedSlotEntrySummaryCount = await Task.WhenAll(tasks);
                return emptyRequestParameters.Concat(processedSlotEntrySummaryCount).ToList();
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, "A timeout error has occurred");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error occurred in trying to execute a GET request");
                throw;
            }
        }

        private Task<SlotEntrySummaryCount> ProcessResults(List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetails, SearchType searchType, DateTime startDate, DateTime endDate, RequestParametersList requestParameter, SemaphoreSlim semaphore, HttpClient client, CancellationToken cancellationToken)
        {
            switch (searchType)
            {
                case SearchType.Provider:
                    if (organisationErrorCodeOrDetails.Where(x => x.providerOrganisation?.OdsCode == requestParameter?.OdsCode)?.FirstOrDefault()?.errorSource == ErrorCode.None)
                    {
                        return PopulateResults(startDate, endDate, requestParameter, semaphore, client, cancellationToken);
                    }
                    break;
                case SearchType.Consumer:
                    if (organisationErrorCodeOrDetails.Where(x => x.consumerOrganisation?.OdsCode == requestParameter?.OdsCode)?.FirstOrDefault()?.errorSource == ErrorCode.None)
                    {
                        return PopulateResults(startDate, endDate, requestParameter, semaphore, client, cancellationToken);
                    }
                    break;
            }
            return null;
        }

        private async Task<SlotEntrySummaryCount> PopulateResults(DateTime startDate, DateTime endDate, RequestParametersList requestParameter, SemaphoreSlim semaphore, HttpClient client, CancellationToken cancellationToken)
        {
            try
            {
                var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x =>
                        x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectSearchFreeSlots);
                requestParameter.RequestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectSearchFreeSlots;
                requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

                AddRequiredRequestHeaders(requestParameter.RequestParameters, client);

                _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                var requestUri = new Uri($"{requestParameter.RequestParameters.EndpointAddressWithSpineSecureProxy}/Slot");
                var uriBuilder = AddQueryParameters(requestParameter.RequestParameters, startDate, endDate, requestUri);

                await semaphore.WaitAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = uriBuilder.Uri
                };

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var contents = await response.Content.ReadAsStringAsync();

                _spineMessage.ResponsePayload = contents;
                _spineMessage.ResponseStatus = response.StatusCode.ToString();
                _spineMessage.RequestPayload = request.ToString();
                _spineMessage.ResponseHeaders = response.Headers.ToString();
                stopWatch.Stop();
                _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                var spineMessage = _logService.AddSpineMessageLog(_spineMessage);

                var results = JsonConvert.DeserializeObject<Bundle>(contents);
                return PopulateSlotEntrySummaryCount(requestParameter, spineMessage, results);
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static SlotEntrySummaryCount PopulateSlotEntrySummaryCount(RequestParametersList requestParameter, DTO.Response.Logging.SpineMessage spineMessage, Bundle results)
        {
            if (results.Issue?.Count > 0)
            {
                return new SlotEntrySummaryCount
                {
                    ErrorCode = ErrorCode.GenericSlotSearchError,
                    ErrorDetail = results.Issue,
                    FreeSlotCount = null,
                    OdsCode = requestParameter.OdsCode,
                    SpineMessageId = spineMessage.SpineMessageId
                };
            }
            else
            {
                return new SlotEntrySummaryCount
                {
                    ErrorCode = ErrorCode.None,
                    ErrorDetail = null,
                    FreeSlotCount = results.entry?.Count(x => x.resource.resourceType == ResourceTypes.Slot),
                    OdsCode = requestParameter.OdsCode,
                    SpineMessageId = spineMessage.SpineMessageId
                };
            }
        }

        public void SendToAudit(List<string> auditSearchParameters, List<string> auditSearchIssues, Stopwatch stopWatch, bool isMultiSearch, int? resultCount = 0)
        {
            var auditEntry = new Entry
            {
                Item1 = auditSearchParameters[0],
                Item2 = auditSearchParameters[1],
                Item3 = auditSearchParameters[2].Replace(":", " "),
                Details = (auditSearchIssues != null && auditSearchIssues.Count > 0) ? string.Join((char)10, auditSearchIssues) : $"{resultCount} free slot(s) returned",
                EntryElapsedMs = Convert.ToInt32(stopWatch.ElapsedMilliseconds),
                EntryTypeId = !isMultiSearch ? (int)AuditEntryType.SingleSlotSearch : (int)AuditEntryType.MultiSlotSearch
            };
            _auditService.AddEntry(auditEntry);
        }
    }
}
