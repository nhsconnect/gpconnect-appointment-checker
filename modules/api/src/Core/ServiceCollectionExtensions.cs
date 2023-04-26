using GpConnect.AppointmentChecker.Api.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Fhir;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;
using GpConnect.AppointmentChecker.Api.Service.Ldap;
using System.Net;
using static GpConnect.AppointmentChecker.Api.Service.OrganisationService;
using static GpConnect.AppointmentChecker.Api.Service.NotificationService;
using DalInterfaces = GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using DalServices = GpConnect.AppointmentChecker.Api.DAL;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services, IConfiguration configuration, IWebHostEnvironment env)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        //services.AddOptions();
        services.Configure<OrganisationServiceConfig>(configuration.GetSection("OrganisationService"));
        services.Configure<NotificationServiceConfig>(configuration.GetSection("NotificationService"));

        services.Configure<Spine>(configuration.GetSection("Spine"));
        services.Configure<General>(configuration.GetSection("General"));
        services.Configure<Sso>(configuration.GetSection("SingleSignOn"));
        services.Configure<Email>(configuration.GetSection("Email"));

        services.AddScoped<DalInterfaces.IDataService, DalServices.DataService>();
        services.AddScoped<IUserService, UserService>();        
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IOrganisationService, OrganisationService>();
        services.AddScoped<IFhirService, FhirService>();
        services.AddScoped<IFhirRequestExecution, FhirRequestExecution>();
        services.AddScoped<ILdapRequestExecution, LdapRequestExecution>();
        services.AddScoped<ILdapService, LdapService>(); 
        services.AddScoped<ISpineService, SpineService>();        
        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<ICapabilityStatement, CapabilityStatement>();
        services.AddScoped<IGpConnectQueryExecutionService, GpConnectQueryExecutionService>();
        services.AddScoped<ISlotSearch, SlotSearch>();
        services.AddScoped<ISlotSearchDependencies, SlotSearchDependencies>();
        services.AddScoped<ISlotSearchFromDatabase, SlotSearchFromDatabase>();
        services.AddScoped<ITokenDependencies, TokenDependencies>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<ISearchService, SearchService>();

        services.AddResponseCaching();
        services.AddResponseCompression();
        services.AddHttpContextAccessor();

        services.AddHealthChecks();
        services.AddControllers();
        services.AddSwaggerGen();

        services.AddHttpClientServices(configuration, env);

        return services;
    }
}

