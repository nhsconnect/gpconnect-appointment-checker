﻿using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Web;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class SlotSearchDependencies : ISlotSearchDependencies
{
    public Practitioner GetPractitionerDetails(string reference, List<RootEntry> scheduleResources, List<RootEntry> practitionerResources)
    {
        var schedule = GetSchedule(reference, scheduleResources);
        var schedulePractitioner = schedule?.resource.actor?.FirstOrDefault(x => x.reference.Contains("Practitioner/"));
        var practitionerRootEntry = practitionerResources?.FirstOrDefault(x => schedulePractitioner?.reference == $"Practitioner/{x.resource.id}")?.resource;
        if (practitionerRootEntry != null)
        {
            var practitioner = new Practitioner
            {
                gender = practitionerRootEntry?.gender,
                name = JsonConvert.DeserializeObject<List<PractitionerName>>(practitionerRootEntry?.name.ToString())
            };
            return practitioner;
        }
        return null;
    }

    public Location GetLocation(string reference, List<RootEntry> scheduleResources, List<RootEntry> locationResources)
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

    public RootEntry GetSchedule(string reference, List<RootEntry> scheduleResources)
    {
        var schedule = scheduleResources.FirstOrDefault(x => reference == $"Schedule/{x.resource.id}");
        return schedule;
    }

    public UriBuilder AddQueryParameters(RequestParameters requestParameters, DateTime startDate, DateTime endDate, Uri requestUri)
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
        if (!string.IsNullOrEmpty(requestParameters.ConsumerODSCode))
        {
            query.Add(Uri.EscapeDataString("searchFilter"), $"https://fhir.nhs.uk/Id/ods-organization-code|{requestParameters.ConsumerODSCode}");
        }
        if (!string.IsNullOrEmpty(requestParameters.GPConnectConsumerOrganisationType))
        { 
            query.Add(Uri.EscapeDataString("searchFilter"), $"https://fhir.nhs.uk/STU3/CodeSystem/GPConnect-OrganisationType-1|{requestParameters.GPConnectConsumerOrganisationType}");
        }
        uriBuilder.Query = query.ToString();
        return uriBuilder;
    }

    public void AddRequiredRequestHeaders(RequestParameters requestParameters, HttpClient client)
    {
        client.DefaultRequestHeaders.Remove("Ssp-From");
        client.DefaultRequestHeaders.Remove("Ssp-To");
        client.DefaultRequestHeaders.Remove("Ssp-InteractionID");
        client.DefaultRequestHeaders.Remove("Ssp-TraceID");
        client.DefaultRequestHeaders.Remove("Bearer");

        client.DefaultRequestHeaders.Add("Ssp-From", requestParameters.SspFrom);
        client.DefaultRequestHeaders.Add("Ssp-To", requestParameters.SspTo);
        client.DefaultRequestHeaders.Add("Ssp-InteractionID", requestParameters.InteractionId);
        client.DefaultRequestHeaders.Add("Ssp-TraceID", Guid.NewGuid().ToString());
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", requestParameters.BearerToken);
    }
}
