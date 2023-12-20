using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ISlotSearchDependencies
{
    Practitioner GetPractitionerDetails(string reference, List<RootEntry> scheduleResources, List<RootEntry> practitionerResources);
    Location GetLocation(string reference, List<RootEntry> scheduleResources, List<RootEntry> locationResources);
    RootEntry GetSchedule(string reference, List<RootEntry> scheduleResources);
    UriBuilder AddQueryParameters(RequestParameters requestParameters, DateTime startDate, DateTime endDate, Uri requestUri);
    void AddRequiredRequestHeaders(RequestParameters requestParameters, HttpClient client);
}
