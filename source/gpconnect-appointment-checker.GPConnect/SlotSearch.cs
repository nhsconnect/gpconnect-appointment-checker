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
                var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

                var response = await client.SendAsync(request);

                var responseStream = await response.Content.ReadAsStringAsync();

                _spineMessage.ResponsePayload = responseStream;
                _spineMessage.ResponseStatus = response.StatusCode.ToString();
                _spineMessage.RequestPayload = request.ToString();
                _spineMessage.ResponseHeaders = response.Headers.ToString();

                stopWatch.Stop();
                _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(_spineMessage);

                var slotSimple = new SlotSimple();
                var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

                if (results.Issue?.Count > 0)
                {
                    slotSimple.Issue = results.Issue;
                    return slotSimple;
                }

                slotSimple.SlotEntrySimple = new List<SlotEntrySimple>();

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
                                    LocationPostalCode = location?.address?.postalCode
                                }).OrderBy(z => z.LocationName)
                    .ThenBy(s => s.AppointmentDate)
                    .ThenBy(s => s.StartTime);
                slotSimple.SlotEntrySimple.AddRange(slotList);
                return slotSimple;
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, "A timeout error has occurred");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
            }
        }

        private List<SlotSimple> GetFreeSlots(List<RequestParametersList> requestParameterList, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            try
            {
                var processedFreeSlots = new ConcurrentBag<SlotSimple>();

                Parallel.ForEach(requestParameterList, requestParameter =>
                {
                    var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                        x.SpineMessageTypeId == (int) SpineMessageTypes.GpConnectSearchFreeSlots);
                    requestParameter.RequestParameters.SpineMessageTypeId = (int) SpineMessageTypes.GpConnectSearchFreeSlots;
                    requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();
                    _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

                    var client = _httpClientFactory.CreateClient("GpConnectClient");

                    client.Timeout = new TimeSpan(0, 0, 30);
                    AddRequiredRequestHeaders(requestParameter.RequestParameters, client);
                    _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                    var requestUri = new Uri($"{AddSecureSpineProxy(requestParameter)}/Slot");
                    var uriBuilder = AddQueryParameters(requestParameter.RequestParameters, startDate, endDate, requestUri);
                    var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

                    using var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    var contents = response.Content.ReadAsStringAsync(cancellationToken).Result;

                    _spineMessage.ResponsePayload = contents;
                    _spineMessage.ResponseStatus = response.StatusCode.ToString();
                    _spineMessage.RequestPayload = request.ToString();
                    _spineMessage.ResponseHeaders = response.Headers.ToString();
                    stopWatch.Stop();
                    _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                    _logService.AddSpineMessageLog(_spineMessage);

                    var slotSimple = new SlotSimple();
                    var results = JsonConvert.DeserializeObject<Bundle>(contents);

                    if (results.Issue?.Count > 0)
                    {
                        slotSimple.Issue = results.Issue;
                        processedFreeSlots.Add(slotSimple);
                    }

                    slotSimple.SlotEntrySimple = new List<SlotEntrySimple>();

                    var slotResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Slot)
                        .ToList();
                    if (slotResources == null || slotResources?.Count == 0) 
                        processedFreeSlots.Add(slotSimple);

                    var practitionerResources = results.entry
                        ?.Where(x => x.resource.resourceType == ResourceTypes.Practitioner).ToList();
                    var locationResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Location)
                        .ToList();
                    var scheduleResources = results.entry?.Where(x => x.resource.resourceType == ResourceTypes.Schedule)
                        .ToList();

                    var slotList = (from slot in slotResources?.Where(s => s.resource != null)
                            let practitioner = GetPractitionerDetails(slot.resource.schedule.reference,
                                scheduleResources, practitionerResources)
                            let location = GetLocation(slot.resource.schedule.reference, scheduleResources,
                                locationResources)
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
                                PractitionerRole = schedule.resource.extension?.FirstOrDefault()?.valueCodeableConcept
                                    ?.coding?.FirstOrDefault()?.display,
                                PractitionerGender = practitioner?.gender,
                                LocationName = location?.name,
                                LocationAddressLines = location?.address?.line,
                                LocationCity = location?.address?.city,
                                LocationCountry = location?.address?.country,
                                LocationDistrict = location?.address?.district,
                                LocationPostalCode = location?.address?.postalCode
                            }).OrderBy(z => z.LocationName)
                        .ThenBy(s => s.AppointmentDate)
                        .ThenBy(s => s.StartTime);
                    slotSimple.SlotEntrySimple.AddRange(slotList);
                    processedFreeSlots.Add(slotSimple);
                });
                return processedFreeSlots.ToList();
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, "A timeout error has occurred");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
            }
        }

        private List<SlotEntrySummaryCount> GetFreeSlotsSummary(List<RequestParametersList> requestParameterList, DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            try
            {
                var processedSlotEntrySummaryCount = new ConcurrentBag<SlotEntrySummaryCount>();

                Parallel.ForEach(requestParameterList, requestParameter =>
                {
                    if (requestParameter.RequestParameters != null)
                    {
                        var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                            x.SpineMessageTypeId == (int) SpineMessageTypes.GpConnectSearchFreeSlots);
                        requestParameter.RequestParameters.SpineMessageTypeId =
                            (int) SpineMessageTypes.GpConnectSearchFreeSlots;
                        requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

                        var stopWatch = new Stopwatch();
                        stopWatch.Start();
                        _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

                        var client = _httpClientFactory.CreateClient("GpConnectClient");

                        client.Timeout = new TimeSpan(0, 0, 30);
                        AddRequiredRequestHeaders(requestParameter.RequestParameters, client);
                        _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                        var requestUri = new Uri($"{AddSecureSpineProxy(requestParameter)}/Slot");
                        var uriBuilder = AddQueryParameters(requestParameter.RequestParameters, startDate, endDate,
                            requestUri);
                        var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);

                        using var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead,
                            cancellationToken);
                        var contents = response.Content.ReadAsStringAsync(cancellationToken).Result;

                        _spineMessage.ResponsePayload = contents;
                        _spineMessage.ResponseStatus = response.StatusCode.ToString();
                        _spineMessage.RequestPayload = request.ToString();
                        _spineMessage.ResponseHeaders = response.Headers.ToString();
                        stopWatch.Stop();
                        _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                        var spineMessage = _logService.AddSpineMessageLog(_spineMessage);

                        var results = JsonConvert.DeserializeObject<Bundle>(contents);

                        if (results.Issue?.Count > 0)
                        {
                            processedSlotEntrySummaryCount.Add(new SlotEntrySummaryCount
                            {
                                ErrorCode = ErrorCode.GenericSlotSearchError,
                                ErrorDetail = results.Issue,
                                FreeSlotCount = null,
                                OdsCode = requestParameter.OdsCode,
                                SpineMessageId = spineMessage.SpineMessageId
                            });
                        }
                        else
                        {
                            processedSlotEntrySummaryCount.Add(new SlotEntrySummaryCount
                            {
                                ErrorCode = ErrorCode.None,
                                ErrorDetail = null,
                                FreeSlotCount = results.entry?.Count(x => x.resource.resourceType == ResourceTypes.Slot),
                                OdsCode = requestParameter.OdsCode,
                                SpineMessageId = spineMessage.SpineMessageId
                            });
                        }
                    }
                    else
                    {
                        processedSlotEntrySummaryCount.Add(new SlotEntrySummaryCount
                        {
                            OdsCode = requestParameter.OdsCode,
                            SpineMessageId = null
                        });
                    }
                });
                return processedSlotEntrySummaryCount.ToList();
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError(timeoutException, "A timeout error has occurred");
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
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
