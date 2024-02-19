using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IOrganisationService
{
    public Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode);
    public Task<Hierarchy[]> GetOrganisationHierarchy(List<string> odsCodes);
    public Task<Hierarchy> GetOrganisationHierarchy(string odsCode, SemaphoreSlim? semaphoreSlim = null);
}
