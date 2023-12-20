using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;

namespace GpConnect.AppointmentChecker.Api.Core;

public static class ApplicationBuilderExtensions
{
    public static void ConfigureApplicationBuilderServices(this IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedProto
        });

        app.UseHsts();
        app.UseRouting();
        app.UseResponseCaching();

        app.Use(async (context, next) =>
        {
            context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
            {
                NoStore = true,
                NoCache = true
            };
            context.Response.Headers.Add("Pragma", "no-cache");
            await next();
        });

        app.UseMiddleware<HeaderCheckMiddleware>();

        app.UseResponseCompression();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks(Helpers.Constants.SystemConstants.HEALTHCHECKERPATH, new HealthCheckOptions()
            {
                ResultStatusCodes =
                {
                    [HealthStatus.Healthy] = StatusCodes.Status200OK,
                    [HealthStatus.Unhealthy] = StatusCodes.Status503ServiceUnavailable
                },
                AllowCachingResponses = false
            });
        });
    }
}