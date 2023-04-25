using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.AWSLambda
{
    public class LambdaConfiguration : ILambdaConfiguration
    {
        public IConfiguration Configuration => new ConfigurationBuilder().Build();
    }
}
