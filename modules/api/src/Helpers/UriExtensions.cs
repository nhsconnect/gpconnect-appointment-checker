namespace GpConnect.AppointmentChecker.Api.Helpers;

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

    public static string GetBaseSiteUrl(this HttpContext httpContext)
    {
        return $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/";
    }

    public static string AddScheme(this string url)
    {
        return !url.StartsWith("https://") ? "https://" + url : url;
    }

    public static string CheckForTrailingSlash(string endpointAddress)
    {
        return !endpointAddress.EndsWith("/") ? endpointAddress : endpointAddress.Substring(0, endpointAddress.Length - 1);
    }
}
