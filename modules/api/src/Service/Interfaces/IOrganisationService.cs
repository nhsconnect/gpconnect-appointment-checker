namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IOrganisationService
{
    public Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode);
    public Task<List<string>> GetOrganisationsFromOdsByRole(string[] roles);
}
