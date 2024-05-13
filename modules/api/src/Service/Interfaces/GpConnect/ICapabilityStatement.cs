using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ICapabilityStatement
{
    Task<CapabilityStatement> GetCapabilityStatement(RequestParameters requestParameters, string baseAddress, string client, string? interactionId = null, TimeSpan? timeoutOverride = null);
}
