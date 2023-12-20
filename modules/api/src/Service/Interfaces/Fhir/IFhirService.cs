using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;

public interface IFhirService
{
    public Task<DTO.Response.Spine.Organisation?> GetOrganisation(string odsCode);
    public Task<Spine> GetProviderDetails(string odsCode);
    public Task<Spine> GetConsumerDetails(string odsCode);
}