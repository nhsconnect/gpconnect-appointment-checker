using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Spine;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface ISpineService
{
    public Task<Organisation> GetOrganisationDetailsByOdsCodeAsync(string odsCode);
    public Task<Spine> GetProviderDetails(string odsCode);
    public Task<Spine> GetConsumerDetails(string odsCode);
}
