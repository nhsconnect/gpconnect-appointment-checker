namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapRequestExecution
    {
        T ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class;
    }
}
