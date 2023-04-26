using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class SlotSearchFromDatabase : ISlotSearchFromDatabase
{
    private readonly ISlotSearchDependencies _slotSearchDependencies;
    private readonly DateTime _currentDateTime = DateTime.Now.TimeZoneConverter();

    public SlotSearchFromDatabase(ISlotSearchDependencies slotSearchDependencies)
    {
        _slotSearchDependencies = slotSearchDependencies;
    }
    
    public SlotSimple GetFreeSlotsFromDatabase(string responseStream)
    {
        var slotSimple = new SlotSimple()
        {
            CurrentSlotEntrySimple = new List<SlotEntrySimple>(),
            PastSlotEntrySimple = new List<SlotEntrySimple>(),
            Issue = new List<Issue>()
        };

        var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

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
        
        return slotSimple;
    }
}