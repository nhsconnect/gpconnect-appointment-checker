using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IHierarchyService
{
    public Task<Hierarchy> GetHierarchyFromSpine(string odsCode);
    public Task<List<Hierarchy>> GetHierarchiesFromSpine(List<string> odsCode);
}
