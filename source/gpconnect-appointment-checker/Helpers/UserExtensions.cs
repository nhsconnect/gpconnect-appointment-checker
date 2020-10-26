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
                if (string.IsNullOrEmpty(claim.Value))
                {
                    return !nullIfEmpty ? defaultValue : claim.Value;
                }
            }
            return nullIfEmpty != true ? defaultValue : null;
        }
    }
}
