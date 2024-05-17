using Amazon.SQS.Model;
using System.Net;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IMessageService
{
    public Task<HttpStatusCode> SendMessageToQueue(SendMessageRequest sendMessageRequest);
}
