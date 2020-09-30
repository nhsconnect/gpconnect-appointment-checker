using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

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

        public async Task<List<SlotSimple>> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Accept", "application/fhir+json");
                client.DefaultRequestHeaders.Add("Ssp-From", requestParameters.SspFrom);
                client.DefaultRequestHeaders.Add("Ssp-To", requestParameters.SspTo);
                client.DefaultRequestHeaders.Add("Ssp-InteractionID", "urn:nhs:names:services:gpconnect:fhir:rest:search:slot-1");
                client.DefaultRequestHeaders.Add("Ssp-TraceID", Guid.NewGuid().ToString());
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestParameters.BearerToken);

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
                if (response.IsSuccessStatusCode)
                {
                    var responseStream = await response.Content.ReadAsStringAsync();
                    var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

                    var organisationResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Organization).ToList();
                    var practitionerResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Practitioner).ToList();
                    var locationResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Location).ToList();
                    var slotResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Slot).ToList();
                    var scheduleResources = results.entry.Where(x => x.resource.resourceType == ResourceTypes.Schedule).ToList();

                    var slotList = (from slot in slotResources
                                    let practitioner = GetPractitionerDetails(slot.resource.schedule.reference, scheduleResources, practitionerResources)
                                    let location = GetLocation(slot.resource.schedule.reference, scheduleResources, locationResources)
                                    let schedule = GetSchedule(slot.resource.schedule.reference, scheduleResources)
                                    select new SlotSimple
                                    {
                                        AppointmentDate = slot.resource.start,
                                        SessionName = schedule.resource.serviceCategory.text,
                                        StartTime = slot.resource.start,
                                        Duration = GetDuration(slot.resource.start, slot.resource.end),
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
                                    }).ToList();
                    return slotList;
                }
                return null;

            }
            catch (Exception exc)
            {
                _logger.LogError("An error occurred in trying to execute a GET request", exc);
                throw;
            }
        }

        private double GetDuration(DateTime? start, DateTime? end)
        {
            if (start == null || end == null) return 0;
            var durationTimeSpan = end - start;
            return durationTimeSpan.Value.TotalMinutes;
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
