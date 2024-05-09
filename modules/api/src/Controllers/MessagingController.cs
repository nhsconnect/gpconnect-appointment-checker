using Amazon.SQS.Model;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

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
    public async Task<IActionResult> PostMessage([FromBody] CompletionRequest completionRequest)
    {
        var request = JsonConvert.SerializeObject(completionRequest, new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            Formatting = Formatting.Indented
        });

        var result = await _service.SendMessageToQueue(new SendMessageRequest()
        {
            MessageBody = request
        });
        return Ok(result);
    }
}
