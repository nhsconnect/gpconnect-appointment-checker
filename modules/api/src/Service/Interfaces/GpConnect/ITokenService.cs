namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ITokenService
{
    Task<DTO.Response.GpConnect.RequestParameters> ConstructRequestParameters(DTO.Request.GpConnect.RequestParameters requestParameters);
}
