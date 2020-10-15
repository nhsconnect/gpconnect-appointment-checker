using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace gpconnect_appointment_checker.Helpers
{
    public static class UserExtensions
    {
        public static string GetClaimValue(this ClaimsPrincipal claims, string claimKey, string defaultValue = "")
        {
            var claim = claims.FindFirst(claimKey);
            return claim != null ? claim.Value : defaultValue;
        }
    }
}
