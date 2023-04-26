using Microsoft.Extensions.Configuration;

namespace gpconnect_appointment_checker.AWSLambda
{
    public interface ILambdaConfiguration
    {
        IConfiguration Configuration { get; }
    }
}