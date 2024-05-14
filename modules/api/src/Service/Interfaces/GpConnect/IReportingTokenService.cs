namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface IReportingTokenService
{
    Task<DTO.Response.GpConnect.RequestParameters> ConstructRequestParameters(DTO.Request.GpConnect.RequestParameters requestParameters, string? interactionId = null, bool isID = true);
}
