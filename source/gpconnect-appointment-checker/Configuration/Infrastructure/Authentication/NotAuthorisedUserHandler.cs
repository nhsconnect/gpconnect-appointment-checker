using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class NotAuthorisedUserHandler : AuthorizationHandler<NotAuthorisedUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, NotAuthorisedUserRequirement requirement)
        {
            if (context.User.GetClaimValue("UserAccountStatus") != null)
            {
                if (context.User.GetClaimValue("UserAccountStatus") != UserAccountStatus.Authorised.ToString())
                {
                    context.Succeed(requirement);
                }
                else
                {
                    var authFilterContext = context.Resource as DefaultHttpContext;
                    authFilterContext.Response.Redirect("/Pending/Index");
                    context.Succeed(requirement);
                }
            }

            return Task.CompletedTask;
        }
    }
}
