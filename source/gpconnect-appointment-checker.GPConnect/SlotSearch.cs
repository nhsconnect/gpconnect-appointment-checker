using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Request.Logging;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.Helpers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        public async Task<SlotSimple> GetFreeSlots(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress)
        {
            try
            {
                var spineMessageType = (await _configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                    x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectSearchFreeSlots);
                requestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectSearchFreeSlots;
                requestParameters.InteractionId = spineMessageType?.InteractionId;

                var stopWatch = new Stopwatch();
                stopWatch.Start();
                _spineMessage.SpineMessageTypeId = requestParameters.SpineMessageTypeId;
                var userSessionId = _context.HttpContext.User.FindFirst("UserSessionId")?.Value;
                if (userSessionId != null) _spineMessage.UserSessionId = Convert.ToInt32(userSessionId);

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

                if (response.IsSuccessStatusCode)
                {
                    var slotSimple = new SlotSimple();
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
                    return slotSimple;
                }
                return null;
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
                name = locationRootEntry?.name.ToString(),
                address = JsonConvert.DeserializeObject<LocationAddress>(locationRootEntry?.address.ToString())
            };
            return location;
        }

        private RootEntry GetSchedule(string reference, List<RootEntry> scheduleResources)
        {
            var schedule = scheduleResources.FirstOrDefault(x => reference == $"Schedule/{x.resource.id}");
            return schedule;
        }

        private static UriBuilder AddQueryParameters(RequestParameters requestParameters, DateTime startDate, DateTime endDate, Uri requestUri)
        {
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
            return uriBuilder;
        }
    }
}
