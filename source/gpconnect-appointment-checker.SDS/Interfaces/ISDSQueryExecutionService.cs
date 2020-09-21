using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ISDSQueryExecutionService
    {
        Task<T> ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class;
    }
}
