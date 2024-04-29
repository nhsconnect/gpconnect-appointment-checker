using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly IWorkflowService _service;

    public WorkflowController(IWorkflowService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet]
    [Route("{odsCode}/{workflowId}")]
    public async Task<IActionResult> GetWorkflowAsync([FromRoute] string odsCode, string workflowId)
    {
        var workflow = await _service.GetWorkflowData(workflowId, odsCode);
        if (workflow == null)
        {
            return NotFound();
        }
        return Ok(workflow);
    }
}
