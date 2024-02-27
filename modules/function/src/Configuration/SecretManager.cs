using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;

namespace GpConnect.AppointmentChecker.Function.Configuration;

public class SecretManager
{
    public string Get(string secretName)
    {
        Console.WriteLine("In SecretManager Get");
        var config = new AmazonSecretsManagerConfig { RegionEndpoint = RegionEndpoint.EUWest2 };
        var client = new AmazonSecretsManagerClient(config);

        var request = new GetSecretValueRequest
        {
            SecretId = $"gpcac/{secretName}"
        };        

        GetSecretValueResponse response = null;
        try
        {
            Console.WriteLine($"Execuiting get for {request.SecretId}");
            response = Task.Run(async () => await client.GetSecretValueAsync(request)).Result;
        }
        catch (ResourceNotFoundException)
        {
            Console.WriteLine("The requested secret " + secretName + " was not found");
        }
        catch (InvalidRequestException e)
        {
            Console.WriteLine("The request was invalid due to: " + e.Message);
        }
        catch (InvalidParameterException e)
        {
            Console.WriteLine("The request had invalid params: " + e.Message);
        }
        return response?.SecretString;
    }    
}
