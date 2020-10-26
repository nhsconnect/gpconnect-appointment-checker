using Microsoft.Extensions.DependencyInjection;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services)
        {
            services.AddSession();
            services.AddControllersWithViews();
            services.AddHttpContextAccessor();
            services.AddRazorPages(options =>
            {
                options.Conventions.AuthorizeFolder("/Private");
                options.Conventions.AllowAnonymousToFolder("/Public");
                options.Conventions.AddPageRoute("/Private/Search", "/Search");
                options.Conventions.AddPageRoute("/Public/AccessDenied", "/AccessDenied");
                options.Conventions.AddPageRoute("/Public/Index", "");
            });
            services.AddAntiforgery(options => { options.SuppressXFrameOptionsHeader = true; });
            return services;
        }
    }
}
