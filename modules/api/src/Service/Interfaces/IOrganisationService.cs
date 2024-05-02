using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IOrganisationService
{
    public Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode);
    public Task<List<Hierarchy>> GetOrganisationHierarchy(List<string> odsCodes);
    public Task<List<string>> GetOrganisationsFromOdsByRole(string[] roles);
    public Task<Hierarchy> GetOrganisationHierarchy(string odsCode);
}
