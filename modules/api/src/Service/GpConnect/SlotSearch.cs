using GpConnect.AppointmentChecker.Api.DTO.Request.Logging;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Newtonsoft.Json;
using System.Diagnostics;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class SlotSearch : ISlotSearch
{
    private readonly ILogger<SlotSearch> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly ISlotSearchDependencies _slotSearchDependencies;
    private readonly ILogService _logService;
    private readonly IHttpClientFactory _httpClientFactory;
    private SpineMessage _spineMessage;
    private readonly DateTime _currentDateTime = DateTime.Now.TimeZoneConverter();

    public SlotSearch(ILogger<SlotSearch> logger, IConfigurationService configurationService, IHttpClientFactory httpClientFactory, ISlotSearchDependencies slotSearchDependencies, ILogService logService)
    {
        _logger = logger;
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _slotSearchDependencies = slotSearchDependencies ?? throw new ArgumentNullException(nameof(slotSearchDependencies));
        _logService = logService ?? throw new ArgumentNullException(nameof(logService));
        _httpClientFactory = httpClientFactory;
        _spineMessage = new SpineMessage();
    }

    public async Task<SlotSimple> GetFreeSlots(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int searchResultId = 0)
    {
        var getRequest = new HttpRequestMessage();

        try
        {
            var spineMessageType = await _configurationService.GetSpineMessageType(SpineMessageTypes.GpConnectSearchFreeSlots);
            requestParameters.SpineMessageTypeId = SpineMessageTypes.GpConnectSearchFreeSlots;
            requestParameters.InteractionId = spineMessageType?.InteractionId;

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            _spineMessage.SpineMessageTypeId = (int)requestParameters.SpineMessageTypeId;

            var client = _httpClientFactory.CreateClient("GpConnectClient");

            client.Timeout = new TimeSpan(0, 0, requestParameters.RequestTimeout);
            _slotSearchDependencies.AddRequiredRequestHeaders(requestParameters, client);
            _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();
            var requestUri = new Uri($"{requestParameters.EndpointAddressWithSpineSecureProxy}/Slot");
            var uriBuilder = _slotSearchDependencies.AddQueryParameters(requestParameters, startDate, endDate, requestUri);

            getRequest.Method = HttpMethod.Get;
            getRequest.RequestUri = uriBuilder.Uri;

            var response = await client.SendAsync(getRequest);

            var responseStream = await response.Content.ReadAsStringAsync();

            _spineMessage.ResponsePayload = responseStream;
            _spineMessage.ResponseStatus = response.StatusCode.ToString();
            _spineMessage.RequestPayload = getRequest.ToString();
            _spineMessage.ResponseHeaders = response.Headers.ToString();
            if (searchResultId > 0) _spineMessage.SearchResultId = searchResultId;

            stopWatch.Stop();
            _spineMessage.RoundTripTimeMs = stopWatch.Elapsed.TotalMilliseconds;
            await _logService.AddSpineMessageLog(_spineMessage);

            var slotSimple = new SlotSimple()
            {
                CurrentSlotEntrySimple = new List<SlotEntrySimple>(),
                PastSlotEntrySimple = new List<SlotEntrySimple>(),
                Issue = new List<Issue>()
            };

            if (responseStream.IsJson())
            {
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
                                let practitioner = _slotSearchDependencies.GetPractitionerDetails(slot.resource.schedule.reference, scheduleResources, practitionerResources)
                                let location = _slotSearchDependencies.GetLocation(slot.resource.schedule.reference, scheduleResources, locationResources)
                                let schedule = _slotSearchDependencies.GetSchedule(slot.resource.schedule.reference, scheduleResources)
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
            }
            else
            {
                slotSimple.Issue.Add(new Issue() { Details = new Detail() { Coding = new List<Coding>() { new() { Code = response.StatusCode.ToString(), Display = "Response was not valid" } } } });
            }
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
}