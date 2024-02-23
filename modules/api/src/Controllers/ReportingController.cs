using GpConnect.AppointmentChecker.Api.DTO.Request;
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
    public async Task<IActionResult> Get([FromRoute] string functionName)
    {
        var report = await _service.GetReport(functionName);
        return Ok(report);
    }

    [HttpPost("export")]
    public async Task<IActionResult> Export([FromBody] ReportRequest reportRequest)
    {
        var result = await _service.ExportReport(reportRequest);
        return SendReport(result);
    }

    [HttpPost("createinteractionreport")]
    public async Task<IActionResult> CreateInteractionReport([FromBody] ReportCreationRequest reportCreationRequest)
    {
        var result = await _service.CreateInteractionReport(reportCreationRequest);
        return Ok(result);
    }

    [HttpPost("createinteractiondata")]
    public async Task<IActionResult> CreateInteractionData([FromBody] ReportInteractionRequest reportInteractionRequest)
    {
        var result = await _service.CreateInteractionData(reportInteractionRequest);
        return Ok(result);
    }

    [HttpPost("createinteractionmessage")]
    public async Task<IActionResult> SendMessageToCreateInteractionReportContentAsync([FromBody] ReportInteractionRequest reportInteractionRequest)
    {
        await _service.SendMessageToCreateInteractionReportContent(reportInteractionRequest);
        return Ok();
    }

    private IActionResult SendReport(Stream result)
    {
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

    [HttpGet("capabilitylist")]
    public async Task<ActionResult> CapabilityList()
    {
        var reports = await _service.GetCapabilityReports();
        return Ok(reports);
    }

    [HttpGet("exportbyspinemessage/{spineMessageId}/{reportName}")]
    public async Task<ActionResult> Export([FromRoute] int spineMessageId, string reportName)
    {
        var report = await _service.ExportBySpineMessage(spineMessageId, reportName);
        return Ok(report);
    }
}
