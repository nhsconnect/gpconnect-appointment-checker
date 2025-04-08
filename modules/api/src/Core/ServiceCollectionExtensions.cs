using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Core.Factories.Interfaces;
using GpConnect.AppointmentChecker.Api.Core.Factories;
using GpConnect.AppointmentChecker.Api.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Api.Dal.Configuration;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Fhir;
using GpConnect.AppointmentChecker.Api.Service.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;
using GpConnect.AppointmentChecker.Api.Service.Ldap;
using System.Net;
using gpconnect_appointment_checker.api.Service;
using gpconnect_appointment_checker.api.Service.Interfaces;
using StackExchange.Redis;
using DalInterfaces = GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using DalServices = GpConnect.AppointmentChecker.Api.DAL;

namespace GpConnect.AppointmentChecker.Api.Core;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureApplicationServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        services.Configure<CookiePolicyOptions>(options =>
        {
            options.CheckConsentNeeded = context => true;
            options.MinimumSameSitePolicy = SameSiteMode.None;
        });

        services.AddOptions();
        services.Configure<ConnectionStrings>(configuration.GetSection("ConnectionStrings"));
        services.Configure<SpineConfig>(configuration.GetSection("SpineConfig"));
        services.Configure<GeneralConfig>(configuration.GetSection("GeneralConfig"));
        services.Configure<NotificationConfig>(configuration.GetSection("NotificationConfig"));
        services.Configure<OrganisationConfig>(configuration.GetSection("OrganisationConfig"));
        services.Configure<SecurityConfig>(configuration.GetSection("SecurityConfig"));
        services.Configure<MessageConfig>(configuration.GetSection("MessageConfig"));
        services.Configure<MeshConfig>(configuration.GetSection("MeshConfig"));
        services.Configure<HierarchyConfig>(configuration.GetSection("HierarchyConfig"));
        services.Configure<CacheConfig>(configuration.GetSection("CacheConfig"));

        services.AddSingleton<ISqsClientFactory, SqsClientFactory>();

        services.AddScoped<DalInterfaces.IDataService, DalServices.DataService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILogService, LogService>();
        services.AddScoped<IAuditService, AuditService>();
        services.AddScoped<IApplicationService, ApplicationService>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IConfigurationService, ConfigurationService>();
        services.AddScoped<IOrganisationService, OrganisationService>();
        services.AddScoped<IHierarchyService, HierarchyService>();
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
        services.AddScoped<IExportService, ExportService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IWorkflowService, WorkflowService>();
        services.AddScoped<IInteractionService, InteractionService>();
        services.AddScoped<IReportingTokenDependencies, ReportingTokenDependencies>();
        services.AddScoped<IReportingTokenService, ReportingTokenService>();

        var redisConnectionString = configuration.GetSection("Redis")["RedisConnectionString"]
                                    ?? throw new Exception("Missing Redis connection string");

        var redis = ConnectionMultiplexer.Connect(redisConnectionString);

        services.AddSingleton<IConnectionMultiplexer>(redis);
        services.AddScoped<ICacheService, RedisCacheService>();


        services.AddResponseCaching();
        services.AddResponseCompression();
        services.AddHttpContextAccessor();

        services.AddHealthChecks();
        services.AddControllers();
        services.AddSwaggerGen();

        services.AddHttpClientServices(configuration);
        services.AddMeshClientServices(configuration);

        services.AddMemoryCacheServices(configuration);

        return services;
    }
}