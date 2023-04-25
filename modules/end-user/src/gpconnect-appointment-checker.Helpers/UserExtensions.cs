using System;
using System.Security.Claims;

namespace gpconnect_appointment_checker.Helpers
{
    public static class UserExtensions
    {
        public static string GetClaimValue(this ClaimsPrincipal claims, string claimKey, string defaultValue = "", bool nullIfEmpty = false)
        {
            var claim = claims.FindFirst(claimKey);
            if (claim != null)
            {
                if (!string.IsNullOrEmpty(claim.Value))
                {
                    return claim.Value;
                }
            }
            return nullIfEmpty != true ? defaultValue : null;
        }

        public static T? GetClaimValue<T>(this ClaimsPrincipal claims, string claimKey) where T : struct
        {
            var claim = claims.FindFirst(claimKey);
            if (claim != null)
            {
                if (!string.IsNullOrEmpty(claim.Value))
                {
                    return (T)Enum.Parse(typeof(T), claim.Value, true);
                }
            }
            return null;
        }

        public static void AddOrReplaceClaimValue(this ClaimsIdentity identity, string claimKey, string replacementValue)
        {
            var claim = identity.FindFirst(claimKey);
            if (claim != null)
            {
                identity.RemoveClaim(claim);
            }
            identity.AddClaim(new Claim(claimKey, replacementValue));
        }
    }
}
