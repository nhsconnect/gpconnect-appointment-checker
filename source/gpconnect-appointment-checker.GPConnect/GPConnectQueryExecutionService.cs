using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.Helpers;

namespace gpconnect_appointment_checker.GPConnect
{
    public class GPConnectQueryExecutionService : IGPConnectQueryExecutionService
    {
        private readonly ILogger<GPConnectQueryExecutionService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;
        private readonly IHttpClientFactory _clientFactory;

        public GPConnectQueryExecutionService(ILogger<GPConnectQueryExecutionService> logger, IConfigurationService configurationService, ILogService logService, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
            _clientFactory = clientFactory;
        }

        public async Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress)
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var loggingSpineMessage = new SpineMessage { SpineMessageTypeId = requestParameters.SpineMessageTypeId };
                var spineMessageType = (await _configurationService.GetSpineMessageTypes()).FirstOrDefault(x => x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectReadMetaData);

                requestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectReadMetaData;
                requestParameters.InteractionId = spineMessageType?.InteractionId;

                var client = _clientFactory.CreateClient();
                AddRequiredRequestHeaders(requestParameters, client);
                loggingSpineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                var requestUri = new Uri($"{AddSecureSpineProxy(baseAddress, requestParameters)}/metadata");

                var uriBuilder = new UriBuilder(requestUri.ToString());
                var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
                var response = await client.SendAsync(request);

                loggingSpineMessage.ResponseStatus = response.StatusCode.ToString();
                loggingSpineMessage.RequestPayload = request.ToString();
                loggingSpineMessage.ResponseHeaders = response.Headers.ToString();

                var responseStream = await response.Content.ReadAsStringAsync();
                loggingSpineMessage.ResponsePayload = responseStream;
                var results = JsonConvert.DeserializeObject<CapabilityStatement>(responseStream);
                stopWatch.Stop();
                loggingSpineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(loggingSpineMessage);
                return results;

            }
            catch (Exception exc)
            {
                _logger.LogError("An error occurred in trying to execute a GET request", exc);
                throw;
            }
        }

        public async Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress)
        {
            try
            {
                var spineMessageType = (await _configurationService.GetSpineMessageTypes()).FirstOrDefault(x => x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectSearchFreeSlots);
                requestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectSearchFreeSlots;
                requestParameters.InteractionId = spineMessageType?.InteractionId;

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var loggingSpineMessage = new SpineMessage { SpineMessageTypeId = requestParameters.SpineMessageTypeId };

                var client = _clientFactory.CreateClient();
                client.Timeout = new TimeSpan(0,0,30);
                AddRequiredRequestHeaders(requestParameters, client);
                loggingSpineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
                var requestUri = new Uri($"{AddSecureSpineProxy(baseAddress, requestParameters)}/Slot");

                var uriBuilder = new UriBuilder(requestUri.ToString());
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query.Add("status", "free");
                query.Add("_include", "Slot:schedule");
                query.Add("_include:recurse", "Schedule:actor:Practitioner");
                query.Add("_include:recurse", "Schedule:actor:Location");
                query.Add("_include:recurse", "Location:managingOrganization");
                query.Add("start", $"ge{startDate:yyyy-MM-dd}");
                query.Add("end", $"le{endDate:yyyy-MM-dd}");
                query.Add("searchFilter", $"https://fhir.nhs.uk/Id/ods-organization-code|{requestParameters.ConsumerODSCode}");
                uriBuilder.Query = query.ToString();

                var request = new HttpRequestMessage(HttpMethod.Get, uriBuilder.Uri);
                var response = await client.SendAsync(request);

                loggingSpineMessage.ResponseStatus = response.StatusCode.ToString();
                loggingSpineMessage.RequestPayload = request.ToString();
                loggingSpineMessage.ResponseHeaders = response.Headers.ToString();

                var slotSimple = new SlotSimple();
                var responseStream = await response.Content.ReadAsStringAsync();
                loggingSpineMessage.ResponsePayload = responseStream;
                var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

                if (results.Issue != null)
                {
                    slotSimple.Issue = results.Issue;
                    return slotSimple;
                }

                slotSimple.SlotEntrySimple = new List<SlotEntrySimple>();

                if (results.entry == null || results.entry.Count == 0) return slotSimple;
                var slotResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Slot).ToList();
                if (slotResources.Count == 0) return slotSimple;

                var practitionerResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Practitioner).ToList();
                var locationResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Location).ToList();
                var scheduleResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Schedule).ToList();

                var slotList = (from slot in slotResources
                                let practitioner = GetPractitionerDetails(slot.resource.schedule.reference, scheduleResources, practitionerResources)
                                let location = GetLocation(slot.resource.schedule.reference, scheduleResources, locationResources)
                                let schedule = GetSchedule(slot.resource.schedule.reference, scheduleResources)
                                select new SlotEntrySimple
                                {
                                    AppointmentDate = slot.resource.start,
                                    SessionName = schedule.resource.serviceCategory.text,
                                    StartTime = slot.resource.start,
                                    Duration = slot.resource.start.DurationBetweenTwoDates(slot.resource.end),
                                    SlotType = slot.resource.serviceType.FirstOrDefault()?.text,
                                    DeliveryChannel = slot.resource.extension.FirstOrDefault()?.valueCode,
                                    PractitionerGivenName = practitioner.name.FirstOrDefault()?.given.FirstOrDefault(),
                                    PractitionerFamilyName = practitioner.name.FirstOrDefault()?.family,
                                    PractitionerPrefix = practitioner.name.FirstOrDefault()?.prefix.FirstOrDefault(),
                                    PractitionerRole = schedule.resource.extension.FirstOrDefault()?.valueCodeableConcept.coding.FirstOrDefault()?.display,
                                    PractitionerGender = practitioner.gender,
                                    LocationName = location.name,
                                    LocationAddressLines = location.address.line,
                                    LocationCity = location.address.city,
                                    LocationCountry = location.address.country,
                                    LocationDistrict = location.address.district,
                                    LocationPostalCode = location.address.postalCode
                                }).OrderBy(z => z.LocationName)
                    .ThenBy(s => s.AppointmentDate)
                    .ThenBy(s => s.StartTime);
                slotSimple.SlotEntrySimple.AddRange(slotList);
                stopWatch.Stop();
                loggingSpineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(loggingSpineMessage);
                return slotSimple;
            }
            catch (TimeoutException timeoutException)
            {
                _logger.LogError("A timeout error has occurred", timeoutException);
                throw;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error occurred in trying to execute a GET request", exc);
                throw;
            }
        }

        private static void AddRequiredRequestHeaders(RequestParameters requestParameters, HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/fhir+json");
            client.DefaultRequestHeaders.Add("Ssp-From", requestParameters.SspFrom);
            client.DefaultRequestHeaders.Add("Ssp-To", requestParameters.SspTo);
            client.DefaultRequestHeaders.Add("Ssp-InteractionID", requestParameters.InteractionId);
            client.DefaultRequestHeaders.Add("Ssp-TraceID", Guid.NewGuid().ToString());
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestParameters.BearerToken);
        }

        private Practitioner GetPractitionerDetails(string reference, List<RootEntry> scheduleResources, List<RootEntry> practitionerResources)
        {
            var schedule = GetSchedule(reference, scheduleResources);
            var schedulePractitioner = schedule?.resource.actor.FirstOrDefault(x => x.reference.Contains("Practitioner/"));
            var practitionerRootEntry = practitionerResources.FirstOrDefault(x => schedulePractitioner?.reference == $"Practitioner/{x.resource.id}")?.resource;
            var practitioner = new Practitioner
            {
                gender = practitionerRootEntry?.gender,
                name = JsonConvert.DeserializeObject<List<PractitionerName>>(practitionerRootEntry?.name.ToString())
            };
            return practitioner;
        }

        private Location GetLocation(string reference, List<RootEntry> scheduleResources, List<RootEntry> locationResources)
        {
            var schedule = GetSchedule(reference, scheduleResources);
            var scheduleLocation = schedule?.resource.actor.FirstOrDefault(x => x.reference.Contains("Location/"));
            var locationRootEntry = locationResources.FirstOrDefault(x => scheduleLocation?.reference == $"Location/{x.resource.id}")?.resource;
            var location = new Location
            {
                name = locationRootEntry.name.ToString(),
                address = JsonConvert.DeserializeObject<LocationAddress>(locationRootEntry?.address.ToString())
            };
            return location;
        }

        private RootEntry GetSchedule(string reference, List<RootEntry> scheduleResources)
        {
            var schedule = scheduleResources.FirstOrDefault(x => reference == $"Schedule/{x.resource.id}");
            return schedule;
        }

        private string AddSecureSpineProxy(string baseAddress, RequestParameters requestParameters)
        {
            return requestParameters.UseSSP ? requestParameters.SspHostname + "/" + baseAddress : baseAddress;
        }
    }
}
