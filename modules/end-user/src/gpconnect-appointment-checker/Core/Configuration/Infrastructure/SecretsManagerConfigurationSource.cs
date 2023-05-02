using Amazon.SecretsManager;
using Microsoft.Extensions.Configuration;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public class SecretsManagerConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var client = CreateClient();
        
        return new SecretsManagerConfigProvider(client);
    }

    private IAmazonSecretsManager CreateClient()
    {
        return new AmazonSecretsManagerClient();
    }
}
