using Microsoft.AspNetCore.Hosting;
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

            
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Private");
                options.Conventions.AllowAnonymousToFolder("/Public");
                options.Conventions.AddPageRoute("/Private/Search", "/Search");
                options.Conventions.AddPageRoute("/Public/Error", "/Error");
                options.Conventions.AddPageRoute("/Public/AccessDenied", "/AccessDenied");
                options.Conventions.AddPageRoute("/Public/Cookies", "/Cookies");
                options.Conventions.AddPageRoute("/Public/Privacy", "/Privacy");
                options.Conventions.AddPageRoute("/Public/Index", "");
            });
            services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });
            HttpClientExtensions.AddHttpClientServices(services, configuration, env);
            return services;
        }

        
    }
}
