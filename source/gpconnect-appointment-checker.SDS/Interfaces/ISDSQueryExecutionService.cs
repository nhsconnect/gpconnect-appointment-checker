namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ISDSQueryExecutionService
    {
        T ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class;
        //T ExecuteLdapQuery<T>(string searchBase, string filter) where T : class;
    }
}
