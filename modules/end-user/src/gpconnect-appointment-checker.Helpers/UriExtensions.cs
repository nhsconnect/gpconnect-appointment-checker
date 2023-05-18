using Microsoft.AspNetCore.Http;
using System;

namespace gpconnect_appointment_checker.Helpers
{
    public static class UriExtensions
    {
        public static Uri GetAbsoluteUri(this HttpContext httpContext)
        {
            var uriBuilder = new UriBuilder();
            uriBuilder.Scheme = httpContext.Request.Scheme;
            uriBuilder.Host = httpContext.Request.Host.Host;
            uriBuilder.Path = httpContext.Request.Path.ToString();
            uriBuilder.Query = httpContext.Request.QueryString.ToString();
            return uriBuilder.Uri;
        }

        public static string GetBaseSiteUrl(this HttpRequest req)
        {
            if (req == null) return null;
            var uriBuilder = new UriBuilder(req.Scheme, req.Host.Host, req.Host.Port ?? -1);
            if (uriBuilder.Uri.IsDefaultPort)
            {
                uriBuilder.Port = -1;
            }

            return uriBuilder.Uri.AbsoluteUri;
        }
    }
}
