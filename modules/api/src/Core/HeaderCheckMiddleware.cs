using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using NLog;

namespace GpConnect.AppointmentChecker.Api.Core;

public class HeaderCheckMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<SecurityConfig> _securityConfig;

    public HeaderCheckMiddleware(RequestDelegate next, IOptions<SecurityConfig> securityConfig)
    {
        _next = next;
        _securityConfig = securityConfig;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        context.Request.Headers.TryGetValue(Headers.ApiKey, out StringValues apiKey);

        if (apiKey.ToString() != _securityConfig.Value.ApiKey)
        {
            context.Response.StatusCode = 401;
        }
        else
        {
            var exemptPaths = new List<string>() { "/log/webrequest", "/spine/organisation", "/user/addOrUpdateUser", "/user/logonUser", "/user/logoffUser", "/user/organisation", "/user/emailaddress" };

            if (!exemptPaths.Any(x => context.Request.Path.StartsWithSegments(x, StringComparison.OrdinalIgnoreCase)))
            {
                context.Request.Headers.TryGetValue(Headers.UserId, out StringValues userId);

                if (!context.Request.Headers.ContainsKey(Headers.UserId))
                {
                    context.Response.StatusCode = 401;
                }
                else
                {
                    context.Items[Headers.UserId] = userId;
                    LogManager.Configuration.Variables[Headers.UserId] = userId.ToString();
                    await _next(context);
                }
            }
            else
            {
                await _next(context);
            }
        }
    }
}