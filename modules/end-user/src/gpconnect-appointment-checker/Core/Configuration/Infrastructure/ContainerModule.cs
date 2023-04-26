using Autofac;
using gpconnect_appointment_checker.Configuration.Infrastructure.Logging.Interface;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            //containerBuilder.RegisterType<FhirRequestExecution>().As<IFhirRequestExecution>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<LdapRequestExecution>().As<ILdapRequestExecution>().InstancePerLifetimeScope();

            //containerBuilder.RegisterType<FhirApiService>().As<IFhirApiService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<LdapService>().As<ILdapService>().InstancePerLifetimeScope();

            //containerBuilder.RegisterType<SdsQueryExecutionBase>().As<ISdsQueryExecutionBase>().InstancePerLifetimeScope();            

            //containerBuilder.RegisterType<GpConnectQueryExecutionService>().As<IGpConnectQueryExecutionService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<DataService>().As<IDataService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<ConfigurationService>().As<IConfigurationService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<AuditService>().As<IAuditService>().InstancePerDependency();
            //containerBuilder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<ApplicationService>().As<IApplicationService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<LogService>().As<ILogService>().InstancePerLifetimeScope();
            //containerBuilder.RegisterType<ReportingService>().As<IReportingService>().InstancePerLifetimeScope();
            containerBuilder.RegisterType<LoggerManager>().As<ILoggerManager>().SingleInstance();            
        }
    }
}