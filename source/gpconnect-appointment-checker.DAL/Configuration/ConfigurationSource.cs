using gpconnect_appointment_checker.DAL.Models;
using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.DAL
{
    public class ConfigurationSource : IConfigurationSource
    {
        public string Configuration { get; set; }
        public string Query { get; set; }

        public ConfigurationSource(ConfigurationOptions options)
        {
            Configuration = options.Configuration;
            Query = options.Query;
        }

        public IConfigurationProvider Build(IConfigurationBuilder builder)
        {
            return new CustomConfigurationProvider(this);
        }
    }
}
