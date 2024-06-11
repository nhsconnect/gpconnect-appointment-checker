using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HierarchyController : ControllerBase
{
    private readonly IHierarchyService _service;

    public HierarchyController(IHierarchyService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet]
    [Route("{odsCode}")]
    public async Task<IActionResult> GetSiteHierarchyAsync([FromRoute] string odsCode)
    {
        var site = await _service.GetHierarchyFromSpine(odsCode);
        if (site == null)
        {
            return NotFound();
        }
        return Ok(site);
    }

    [HttpPost]
    public async Task<IActionResult> GetOrganisationHierarchiesAsync([FromBody] List<string> odsCodes)
    {
        var result = await _service.GetHierarchiesFromSpine(odsCodes);
        return Ok(result);
    }
}
