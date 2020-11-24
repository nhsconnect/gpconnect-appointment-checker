using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapTokenService
    {
        Task ExecutionTokenValidation(TokenValidatedContext context);
    }
}
