using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
        {
            services.AddSession(s =>
            {
                s.IdleTimeout = new System.TimeSpan(0, 30, 0);
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddResponseCompression();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Private");
                options.Conventions.AllowAnonymousToFolder("/Public");
                options.Conventions.AddPageRoute("/Private/Search", "/Search");
                options.Conventions.AddPageRoute("/Public/Error", "/Error");
                options.Conventions.AddPageRoute("/Public/AccessDenied", "/AccessDenied");
                options.Conventions.AddPageRoute("/Public/Accessibility", "/Accessibility");
                options.Conventions.AddPageRoute("/Public/Help", "/Help");
                options.Conventions.AddPageRoute("/Public/Index", "");
            });
            services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });
            HttpClientExtensions.AddHttpClientServices(services, configuration, env);
            return services;
        }


    }
}
