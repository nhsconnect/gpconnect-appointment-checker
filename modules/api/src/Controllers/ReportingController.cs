using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
    public async Task<ActionResult> Get([FromRoute] string functionName)
    {
        var report = await _service.GetReport(functionName);
        return Ok(report);
    }

    [HttpGet("exportbyreportname/{functionName}/{reportName}")]
    public async Task<ActionResult> Export([FromRoute] string functionName, string reportName)
    {
        var report = await _service.ExportByReportName(functionName, reportName);
        return Ok(report);
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
