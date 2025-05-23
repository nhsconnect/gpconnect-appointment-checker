using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthorisedOrNotAuthorisedUserHandler : AuthorizationHandler<AuthorisedOrNotAuthorisedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorisedOrNotAuthorisedUserRequirement requirement)
        {
            var authFilterContext = context.Resource as DefaultHttpContext;

            if (!string.IsNullOrEmpty(context.User.GetClaimValue("UserAccountStatus")))
            {
                if (context.User.GetClaimValue("UserAccountStatus") == UserAccountStatus.Pending.ToString())
                {
                    authFilterContext.Response.Redirect("/PendingAccount");
                    context.Succeed(requirement);
                }
                else
                {
                    context.Succeed(requirement);
                }
            }
            else
            {                
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
