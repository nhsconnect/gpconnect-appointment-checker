using GpConnect.AppointmentChecker.Api.Helpers.Constants;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;

public interface IFhirRequestExecution
{
    Task<T> ExecuteFhirQuery<T>(string query, string baseAddress, CancellationToken cancellationToken, SpineMessageTypes spineMessageType = SpineMessageTypes.SpineFhirApiSDSQuery) where T : class;
}
