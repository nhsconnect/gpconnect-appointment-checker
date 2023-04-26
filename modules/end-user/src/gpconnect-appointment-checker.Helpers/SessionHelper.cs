using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace gpconnect_appointment_checker.Helpers
{
    public static class SessionHelper
    {
        public static string GetSessionId(this HttpContext context)
        {
            var hasSession = context.Features.Get<ISessionFeature>()?.Session != null;
            return hasSession ? context.Session != null ? context.Session.Id : string.Empty : string.Empty;
        }
    }
}
