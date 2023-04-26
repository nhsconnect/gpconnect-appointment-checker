using Microsoft.AspNetCore.Authorization;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthorisedUserRequirement : IAuthorizationRequirement
    {
        public AuthorisedUserRequirement()
        {            
        }
    }
}
