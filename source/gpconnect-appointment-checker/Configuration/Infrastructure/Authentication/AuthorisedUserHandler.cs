using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthorisedUserHandler : AuthorizationHandler<AuthorisedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorisedUserRequirement requirement)
        {
            if (!string.IsNullOrEmpty(context.User.GetClaimValue("UserAccountStatus")))
            {
                if (context.User.GetClaimValue("UserAccountStatus") == UserAccountStatus.Authorised.ToString())
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var authFilterContext = context.Resource as DefaultHttpContext;
                    authFilterContext.Response.Redirect("/Index");
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
