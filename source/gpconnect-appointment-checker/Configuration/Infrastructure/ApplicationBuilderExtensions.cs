using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Net.Http.Headers;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureApplicationBuilderServices(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseExceptionHandler("/Error");

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseHsts();

            app.UseHttpsRedirection();

            app.UseStaticFiles(new StaticFileOptions
            {
                OnPrepareResponse = context =>
                {
                    context.Context.Response.Headers[HeaderNames.CacheControl] = $"public, max-age={TimeSpan.FromSeconds(60 * 60 * 24)}";
                }
            });
            app.UseSession();
            app.UseCookiePolicy();

            app.UseStatusCodePagesWithRedirects("/StatusCode/{0}");
            app.UseRouting();            

            app.UseResponseCaching();

            app.Use(async (context, next) =>
            {
                context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue()
                {
                    NoStore = true,
                    NoCache = true
                };
                AddResponseHeaders(context);
                await next(context);
            });

            app.UseResponseCompression();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<RequestLoggingMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
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

        private static void AddResponseHeaders(HttpContext context)
        {
            if (!context.Response.Headers.ContainsKey("Pragma"))
            {
                context.Response.Headers.Add("Pragma", "no-cache");
            }
            if (!context.Response.Headers.ContainsKey("X-Frame-Options"))
            {
                context.Response.Headers.Add("X-Frame-Options", "SAMEORIGIN");
            }
        }
    }
}