using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class LogController : ControllerBase
{
    private readonly ILogService _service;

    public LogController(ILogService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpPost("webrequest")]
    public async Task<IActionResult> WebRequest([FromBody] DTO.Request.Logging.WebRequest request)
    {
        await _service.AddWebRequestLog(request);
        return Ok();
    }

    [HttpPost("spinemessage")]
    public async Task<IActionResult> SpineMessage([FromBody] DTO.Request.Logging.SpineMessage request)
    {
        await _service.AddSpineMessageLog(request);
        return Ok();
    }

    [HttpPost("errorlog")]
    public async Task<IActionResult> ErrorLog([FromBody] DTO.Request.Logging.ErrorLog request)
    {
        await _service.AddErrorLog(request);
        return Ok();
    }
}
