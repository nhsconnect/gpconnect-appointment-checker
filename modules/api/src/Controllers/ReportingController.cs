using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ReportingController : ControllerBase
{
    private readonly IReportingService _service;

    public ReportingController(IReportingService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("{functionName}")]
    public async Task<IActionResult> Get([FromRoute] string functionName)
    {
        var report = await _service.GetReport(functionName);
        return Ok(report);
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ReportRequest reportRequest)
    {
        var result = await _service.ExportReport(reportRequest);
        if (result == null)
        {
            return NotFound();
        }
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [HttpGet("list")]
    public async Task<ActionResult> List()
    {
        var reports = await _service.GetReports();
        return Ok(reports);
    }

    [HttpGet("exportbyspinemessage/{spineMessageId}/{userId}/{reportName}")]
    public async Task<ActionResult> Export([FromRoute] int spineMessageId, int userId, string reportName)
    {
        var report = await _service.ExportBySpineMessage(spineMessageId, userId, reportName);
        return Ok(report);
    }
}
