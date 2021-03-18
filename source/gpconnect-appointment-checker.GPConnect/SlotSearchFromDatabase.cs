using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.Helpers;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        private SlotSimple GetFreeSlotsFromDatabase(string responseStream)
        {
            var slotSimple = new SlotSimple();
            var results = JsonConvert.DeserializeObject<Bundle>(responseStream);

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
                                AppointmentDate = slot.resource.start,
                                SessionName = schedule.resource.serviceCategory?.text,
                                StartTime = slot.resource.start,
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
    }
}