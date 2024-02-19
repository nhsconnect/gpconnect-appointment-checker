using Amazon;
using Amazon.SQS;
using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Core.Factories.Interfaces;
using Microsoft.Extensions.Options;

namespace GpConnect.AppointmentChecker.Api.Core.Factories;

public class SqsClientFactory : ISqsClientFactory
{
    private readonly IOptions<MessageConfig> _config;

    public SqsClientFactory(IOptions<MessageConfig> config)
    {
        _config = config;
    }

    public IAmazonSQS? GetSqsClient()
    {
        var config = new AmazonSQSConfig
        {
            RegionEndpoint = RegionEndpoint.GetBySystemName(_config.Value.RegionEndpoint),
            ServiceURL = _config.Value.ServiceURL            
        };
        if (_config.Value.SendEnabled)
        {
            return new AmazonSQSClient(config);
        }
        return null;
    }

    public string GetSqsQueue() => _config.Value.ReportingQueueUrl;
}
