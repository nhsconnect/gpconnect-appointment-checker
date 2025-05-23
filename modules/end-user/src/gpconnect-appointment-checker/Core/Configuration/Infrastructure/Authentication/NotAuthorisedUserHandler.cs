using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class NotAuthorisedUserHandler : AuthorizationHandler<NotAuthorisedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NotAuthorisedUserRequirement requirement)
        {
            if (!string.IsNullOrEmpty(context.User.GetClaimValue("UserAccountStatus")))
            {
                if (context.User.GetClaimValue("UserAccountStatus") != UserAccountStatus.Authorised.ToString())
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
