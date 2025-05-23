using Microsoft.AspNetCore.Authentication.Cookies;
using System;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    internal static class PrincipalValidator
    {
        internal static Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            var userId = context.Principal.GetClaimValue("UserId");

            if (userId == null)
            {
                context.RejectPrincipal();
            }
            
            context.Options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            
            return Task.CompletedTask;
        }
    }
}
