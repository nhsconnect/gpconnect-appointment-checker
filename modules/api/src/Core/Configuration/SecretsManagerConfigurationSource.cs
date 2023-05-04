using Amazon;
using Amazon.SecretsManager;

namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class SecretsManagerConfigurationSource : IConfigurationSource
{
    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        var client = CreateClient();
        
        return new SecretsManagerConfigProvider(client);
    }

    private IAmazonSecretsManager CreateClient()
    {
        return new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName("eu-west-2"));
    }
}
