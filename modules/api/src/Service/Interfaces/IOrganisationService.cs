namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IOrganisationService
{
    public Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode);
}
