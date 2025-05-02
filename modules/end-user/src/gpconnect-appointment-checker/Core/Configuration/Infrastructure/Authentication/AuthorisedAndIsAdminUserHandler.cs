using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Configuration.Infrastructure.Authentication
{
    public class AuthorisedAndIsAdminUserHandler : AuthorizationHandler<AuthorisedAndIsAdminUserRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthorisedAndIsAdminUserRequirement requirement)
        {
            var authFilterContext = context.Resource as DefaultHttpContext;

            if (!string.IsNullOrEmpty(context.User.GetClaimValue("UserAccountStatus")))
            {
                if (context.User.GetClaimValue("UserAccountStatus") == UserAccountStatus.Authorised.ToString() && context.User.GetClaimValue("IsAdmin").StringToBoolean(false))
                {
                    context.Succeed(requirement);
                }
                else
                {
                    authFilterContext.Response.StatusCode = StatusCodes.Status404NotFound;
                    context.Fail();
                }
            }
            else
            {
                authFilterContext.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Fail();
            }

            return Task.CompletedTask;
        }
    }
}
