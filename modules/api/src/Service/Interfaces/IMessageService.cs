using Amazon.SQS.Model;
using GpConnect.AppointmentChecker.Api.DTO.Response.Message;
using System.Net;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IMessageService
{
    public Task<HttpStatusCode> SendMessageToQueue(SendMessageRequest sendMessageRequest);
    public Task<MessageStatus> GetMessageStatus();
}
