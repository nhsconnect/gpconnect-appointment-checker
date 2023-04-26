namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;

public interface ILdapRequestExecution
{
    T ExecuteLdapQuery<T>(string searchBase, string filter, string[] attributes) where T : class;
}
