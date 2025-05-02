using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthorisedUserHandler : AuthorizationHandler<AuthorisedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorisedUserRequirement requirement)
        {
            var authFilterContext = context.Resource as DefaultHttpContext;

            if (!string.IsNullOrEmpty(context.User.GetClaimValue("UserAccountStatus")))
            {
                if (context.User.GetClaimValue("UserAccountStatus") == UserAccountStatus.Authorised.ToString())
                {
                    context.Succeed(requirement);
                }
                else if (context.User.GetClaimValue("UserAccountStatus") == UserAccountStatus.Pending.ToString())
                {
                    authFilterContext.Response.Redirect("/PendingAccount");
                    context.Succeed(requirement);
                }
                else
                {
                    authFilterContext.Response.Redirect("/Index");
                    context.Fail();
                }
            }
            else
            {
                if (!context.User.Identity.IsAuthenticated)
                {
                    authFilterContext.Response.Redirect("/Index");
                    context.Fail();
                }
                else
                {
                    authFilterContext.Response.Redirect("/Index");
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
