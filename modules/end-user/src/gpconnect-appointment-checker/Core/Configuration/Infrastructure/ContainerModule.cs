using Autofac;
using GpConnect.AppointmentChecker.Core.HttpClientServices;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

namespace gpconnect_appointment_checker.Core.Configuration.Infrastructure
{
    public class ContainerModule : Module
    {
        protected override void Load(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<TokenService>().As<ITokenService>().InstancePerLifetimeScope();      
        }
    }
}
