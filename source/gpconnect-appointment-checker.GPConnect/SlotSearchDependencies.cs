using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        protected static Practitioner GetPractitionerDetails(string reference, List<RootEntry> scheduleResources, List<RootEntry> practitionerResources)
        {
            var schedule = GetSchedule(reference, scheduleResources);
            var schedulePractitioner = schedule?.resource.actor?.FirstOrDefault(x => x.reference.Contains("Practitioner/"));
            var practitionerRootEntry = practitionerResources?.FirstOrDefault(x => schedulePractitioner?.reference == $"Practitioner/{x.resource.id}")?.resource;
            var practitioner = new Practitioner
            {
                gender = practitionerRootEntry?.gender,
                name = JsonConvert.DeserializeObject<List<PractitionerName>>(practitionerRootEntry?.name.ToString())
            };
            return practitioner;
        }

        protected static Location GetLocation(string reference, List<RootEntry> scheduleResources, List<RootEntry> locationResources)
        {
            var schedule = GetSchedule(reference, scheduleResources);
            var scheduleLocation = schedule?.resource.actor?.FirstOrDefault(x => x.reference.Contains("Location/"));
            var locationRootEntry = locationResources?.FirstOrDefault(x => scheduleLocation?.reference == $"Location/{x.resource.id}")?.resource;
            var location = new Location
            {
                name = locationRootEntry?.name.ToString(),
                address = locationRootEntry?.address != null ? JsonConvert.DeserializeObject<LocationAddress>(locationRootEntry.address.ToString()) : null
            };
            return location;
        }

        protected static RootEntry GetSchedule(string reference, List<RootEntry> scheduleResources)
        {
            var schedule = scheduleResources.FirstOrDefault(x => reference == $"Schedule/{x.resource.id}");
            return schedule;
        }

        protected static UriBuilder AddQueryParameters(RequestParameters requestParameters, DateTime startDate, DateTime endDate, Uri requestUri)
        {
            var uriBuilder = new UriBuilder(requestUri.ToString());
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query.Add(Uri.EscapeDataString("status"), "free");
            query.Add(Uri.EscapeDataString("_include"), "Slot:schedule");
            query.Add(Uri.EscapeDataString("_include:recurse"), "Schedule:actor:Practitioner");
            query.Add(Uri.EscapeDataString("_include:recurse"), "Schedule:actor:Location");
            query.Add(Uri.EscapeDataString("_include:recurse"), "Location:managingOrganization");
            query.Add(Uri.EscapeDataString("start"), $"ge{startDate:yyyy-MM-dd}");
            query.Add(Uri.EscapeDataString("end"), $"le{endDate:yyyy-MM-dd}");
            query.Add(Uri.EscapeDataString("searchFilter"), $"https://fhir.nhs.uk/Id/ods-organization-code|{requestParameters.ConsumerODSCode}");
            uriBuilder.Query = query.ToString();
            return uriBuilder;
        }
    }
}
