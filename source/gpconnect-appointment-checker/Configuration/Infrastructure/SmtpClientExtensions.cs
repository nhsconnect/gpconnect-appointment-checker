using System;
using System.Net;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using gpconnect_appointment_checker.Helpers;

namespace gpconnect_appointment_checker.Configuration.Infrastructure
{
    public static class SmtpClientExtensions
    {
        public static void AddSmtpClientServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped(serviceProvider => new SmtpClient
            {
                Host = configuration.GetSection("Email:host_name").Value,
                Port = configuration.GetSection("Email:port").Value.StringToInteger(),
                DeliveryFormat = SmtpDeliveryFormat.SevenBit,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                EnableSsl = GetEncryption(configuration),
                Credentials = GetCredentials(configuration)
            });
        }

        private static bool GetEncryption(IConfiguration configuration)
        {
            var encryptionMethod = configuration.GetSection("Email:encryption").Value;
            return Enum.TryParse(typeof(SecurityProtocolType), encryptionMethod, true, out _);
        }

        private static ICredentialsByHost GetCredentials(IConfiguration configuration)
        {
            if (configuration.GetSection("Email:authentication_required").Value.StringToBoolean(false))
            {
                return new NetworkCredential
                {
                    UserName = configuration.GetSection("Email:user_name").Value,
                    Password = configuration.GetSection("Email:password").Value
                };
            }
            return null;
        }
    }
}
