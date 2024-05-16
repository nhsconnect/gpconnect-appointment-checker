using Amazon.SQS;

namespace GpConnect.AppointmentChecker.Api.Core.Factories.Interfaces;

public interface ISqsClientFactory
{
    public IAmazonSQS? GetSqsClient();
    public string GetSqsQueue();
    public string GetSqsOutputQueue();
}
