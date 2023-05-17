using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExportController : ControllerBase
{
    private readonly IExportService _service;

    public ExportController(IExportService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpPost("exportsearchresult")]
    public async Task<IActionResult> ExportSearchResultFromDatabase([FromBody] ExportRequest request)
    {
        var result = await _service.ExportSearchResultFromDatabase(request);
        if (result == null)
        {
            return NotFound();
        }        
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }

    [HttpPost("exportsearchgroup")]
    public async Task<IActionResult> ExportSearchGroupFromDatabaseRequest([FromBody] ExportRequest request)
    {
        var result = await _service.ExportSearchGroupFromDatabase(request);
        if (result == null)
        {
            return NotFound();
        }
        return File(result, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
    }
}
