using gpconnect_appointment_checker.GPConnect.Constants;
using System.Threading;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface IFhirRequestExecution
    {
        Task<T> ExecuteFhirQuery<T>(string query, string baseAddress, CancellationToken cancellationToken, SpineMessageTypes spineMessageType = SpineMessageTypes.SpineFhirApiSDSQuery) where T : class;
    }
}
