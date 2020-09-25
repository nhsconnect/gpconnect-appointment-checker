using System.Net.Http;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface IGPConnectQueryExecutionService
    {
        Task<T> ExecuteGet<T>(HttpRequestMessage request) where T : class;
    }
}
