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
                var requestUri = new Uri($"{AddSecureSpineProxy(baseAddress, requestParameters)}/Slot");
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
                var tasks = new ConcurrentBag<Task<SlotEntrySummaryCount>>();

                Parallel.ForEach(requestParameterList.Where(x => x.RequestParameters == null), requestParameter =>
                {
                    tasks.Add(Task.FromResult(new SlotEntrySummaryCount
                    {
                        OdsCode = requestParameter.OdsCode,
                        SpineMessageId = null
                    }));
                });

                Parallel.ForEach(requestParameterList.Where(x => x.RequestParameters != null), requestParameter =>
                {
                    switch (searchType)
                    {
                        case SearchType.Provider:
                            if (organisationErrorCodeOrDetails.Where(x => x.providerOrganisation?.ODSCode == requestParameter?.OdsCode)?.FirstOrDefault()?.errorSource == ErrorCode.None)
                            {
                                tasks.Add(Task.FromResult(PopulateResults(startDate, endDate, requestParameter, cancellationToken)));
                            }
                            break;
                        case SearchType.Consumer:
                            if (organisationErrorCodeOrDetails.Where(x => x.consumerOrganisation?.ODSCode == requestParameter?.OdsCode)?.FirstOrDefault()?.errorSource == ErrorCode.None)
                            {
                                tasks.Add(Task.FromResult(PopulateResults(startDate, endDate, requestParameter, cancellationToken)));
                            }
                            break;
                    }
                });

                var processedSlotEntrySummaryCount = await Task.WhenAll(tasks);
                return processedSlotEntrySummaryCount.ToList();
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

        private SlotEntrySummaryCount PopulateResults(DateTime startDate, DateTime endDate, RequestParametersList requestParameter, CancellationToken cancellationToken)
        {
            var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x =>
                    x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectSearchFreeSlots);
            requestParameter.RequestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectSearchFreeSlots;
            requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

            var client = _httpClientFactory.CreateClient("GpConnectClient");

            AddRequiredRequestHeaders(requestParameter.RequestParameters, client);
            _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
            var requestUri = new Uri($"{AddSecureSpineProxy(requestParameter)}/Slot");
            var uriBuilder = AddQueryParameters(requestParameter.RequestParameters, startDate, endDate, requestUri);

            var getRequest = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
            var response = client.Send(getRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            var contents = response.Content.ReadAsStringAsync(cancellationToken).Result;

            _spineMessage.ResponsePayload = contents;
            _spineMessage.ResponseStatus = response.StatusCode.ToString();
            _spineMessage.RequestPayload = getRequest.ToString();
            _spineMessage.ResponseHeaders = response.Headers.ToString();
            stopWatch.Stop();
            _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
            var spineMessage = _logService.AddSpineMessageLog(_spineMessage);

            var results = JsonConvert.DeserializeObject<Bundle>(contents);

            return PopulateSlotEntrySummaryCount(requestParameter, spineMessage, results);
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
