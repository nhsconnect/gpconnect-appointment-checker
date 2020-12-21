using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.SDS.Interfaces
{
    public interface ILdapTokenService
    {
        Task ExecutionTokenValidation(TokenValidatedContext context);
    }
}
