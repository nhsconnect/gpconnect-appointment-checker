using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class ApplicationBuilderExtensions
    {
        public static void ConfigureApplicationBuilderServices(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            //app.UseExceptionHandler(err => err.ErrorHandlingMiddleware(env));
            app.UseExceptionHandler("/Error");
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseResponseCompression();

            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedProto
            });

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}