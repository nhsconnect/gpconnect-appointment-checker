using Amazon.SQS.Model;
using GpConnect.AppointmentChecker.Api.Core.Factories.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response.Message;
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
        return await SendMessage(sendMessageRequest);
    }

    public async Task<HttpStatusCode> SendMessageToOutputQueue(SendMessageRequest sendMessageRequest)
    {
        sendMessageRequest.QueueUrl = _sqsClientFactory.GetSqsOutputQueue();

        _logger.LogInformation("Sending message to output queue: " + sendMessageRequest.QueueUrl);
        _logger.LogInformation("Sending message: " + sendMessageRequest.MessageBody);

        return await SendMessage(sendMessageRequest);
    }

    private async Task<HttpStatusCode> SendMessage(SendMessageRequest sendMessageRequest)
    {
        var sqsClient = _sqsClientFactory.GetSqsClient();
        if (sqsClient != null)
        {
            var response = await sqsClient.SendMessageAsync(sendMessageRequest, CancellationToken.None);
            return response.HttpStatusCode;
        }
        return HttpStatusCode.ServiceUnavailable;
    }

    public async Task<MessageStatus> GetMessageStatus()
    {
        var messageStatus = new MessageStatus()
        {
            MessagesInFlight = -1,
            MessagesAvailable = -1,
        };
        var sqsClient = _sqsClientFactory.GetSqsClient();
        if (sqsClient != null)
        {            
            var queueUrl = _sqsClientFactory.GetSqsQueue();
            var queueAttributes = await sqsClient.GetQueueAttributesAsync(queueUrl, new List<string> { "ApproximateNumberOfMessages", "ApproximateNumberOfMessagesNotVisible" });
            if (queueAttributes != null)
            {
                messageStatus.MessagesAvailable = queueAttributes.ApproximateNumberOfMessages;
                messageStatus.MessagesInFlight = queueAttributes.ApproximateNumberOfMessagesNotVisible;
            }
        }
        return messageStatus;
    }
}
