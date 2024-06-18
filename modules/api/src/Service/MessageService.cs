using Amazon.SQS.Model;
using GpConnect.AppointmentChecker.Api.Core.Factories.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using System.Net;

namespace GpConnect.AppointmentChecker.Api.Service;

public class MessageService : IMessageService
{
    private readonly ISqsClientFactory _sqsClientFactory;
    private readonly ILogger<MessageService> _logger;

    public MessageService(ISqsClientFactory sqsClientFactory, ILogger<MessageService> logger)
    {
        _sqsClientFactory = sqsClientFactory ?? throw new ArgumentNullException(nameof(sqsClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<HttpStatusCode> SendMessageToQueue(SendMessageRequest sendMessageRequest)
    {
        sendMessageRequest.QueueUrl = _sqsClientFactory.GetSqsQueue();
        var sqsClient = _sqsClientFactory.GetSqsClient();
        if (sqsClient != null)
        {
            var response = await sqsClient.SendMessageAsync(sendMessageRequest, CancellationToken.None);
            return response.HttpStatusCode;
        }
        return HttpStatusCode.ServiceUnavailable;
    }

    public async Task<HttpStatusCode> SendMessageBatchToQueue(SendMessageBatchRequest sendMessageBatchRequest)
    {
        sendMessageBatchRequest.QueueUrl = _sqsClientFactory.GetSqsQueue();
        var sqsClient = _sqsClientFactory.GetSqsClient();
        if (sqsClient != null)
        {
            var response = await sqsClient.SendMessageBatchAsync(sendMessageBatchRequest, CancellationToken.None);

            if (response.Successful.Count > 0)
            {
                foreach (var success in response.Successful)
                {
                    _logger.LogInformation(success.Id);
                    _logger.LogInformation(success.MessageId);
                    _logger.LogInformation(success.MD5OfMessageBody);
                }
            }

            if (response.Failed.Count > 0)
            {
                foreach (var fail in response.Failed)
                {
                    _logger.LogInformation(fail.Id);
                    _logger.LogInformation(fail.Code);
                    _logger.LogInformation(fail.Message);
                    _logger.LogInformation(fail.SenderFault.ToString());
                }
            }

            return response.HttpStatusCode;
        }
        return HttpStatusCode.ServiceUnavailable;
    }
}
