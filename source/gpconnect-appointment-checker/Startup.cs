using gpconnect_appointment_checker.Logging.GlobalExceptionHandler;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gpconnect_appointment_checker
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddRazorPages();
            services.AddAntiforgery(options =>
            {
                options.SuppressXFrameOptionsHeader = true;
            });
            services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                        options.DefaultChallengeScheme = "GpConnectAppointmentChecker";
                    }).AddCookie()
                .AddOAuth("NHS-SSO", options =>
                {
                    options.ClientId = System.Guid.NewGuid().ToString(); //Configuration["GpConnectAppointmentChecker:ClientId"];
                    options.ClientSecret = "icuJ3Ppo$5$X6HRTDx@g"; //Configuration["GpConnectAppointmentChecker:ClientSecret"];
                    options.CallbackPath = new PathString("/signin-nhsmail");
                    options.AuthorizationEndpoint = "/Auth";
                    options.TokenEndpoint = "/Token";
                });
            services.AddControllers();
            services.AddSingleton<ILog, LogNLog>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, ILog logger, IWebHostEnvironment env, IHttpContextAccessor contextAccessor)
        {
            app.ConfigureExceptionHandler(logger);
            app.UseHsts();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication(); 
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
            });
        }
    }
}
