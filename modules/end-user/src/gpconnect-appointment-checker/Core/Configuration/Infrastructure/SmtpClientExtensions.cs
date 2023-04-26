using gpconnect_appointment_checker.DTO.Response.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public class SmtpClientExtensions
    {
        public Email _emailConfig { get; private set; }

        public SmtpClientExtensions(IConfiguration config)
        {
            _emailConfig = config.GetSection("Email").Get<Email>();
        }

        public void AddSmtpClientServices(IServiceCollection services)
        {
            services.AddScoped(serviceProvider => new SmtpClient
            {
                Host = _emailConfig.HostName,
                Port = _emailConfig.Port,
                DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = true,
                Credentials = new System.Net.NetworkCredential
                {
                    UserName = _emailConfig.UserName,
                    Password = _emailConfig.Password
                }
            });
        }
    }
}
