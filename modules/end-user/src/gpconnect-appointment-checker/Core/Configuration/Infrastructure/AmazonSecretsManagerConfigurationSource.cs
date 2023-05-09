using Microsoft.Extensions.Configuration;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public class AmazonSecretsManagerConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new AmazonSecretsManagerConfigurationProvider();
    }

}
