using Amazon.SQS.Model;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagingController : ControllerBase
{
    private readonly IMessageService _service;

    public MessagingController(IMessageService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("getmessagestatus")]
    public async Task<IActionResult> GetMessageStatus()
    {
        var result = await _service.GetMessageStatus();
        return Ok(result);
    }

    [HttpPost]
    public async Task<IActionResult> SendMessageToQueue([FromBody] string messageBody)
    {        
        var request = new SendMessageRequest { MessageBody = messageBody };
        var response = await _service.SendMessageToQueue(request);
        return Ok(response);
    }
}
