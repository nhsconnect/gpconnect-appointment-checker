using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IOrganisationService
{
    public Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode);
    public Task<Dictionary<string, Hierarchy>> GetOrganisationHierarchy(List<string> odsCodes);
    public Task<Hierarchy> GetOrganisationHierarchy(string odsCode);
}
