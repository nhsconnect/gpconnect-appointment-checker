using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class OrganisationController : ControllerBase
{
    private readonly IOrganisationService _service;

    public OrganisationController(IOrganisationService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("{odsCode}", Name = "GetOrganisationAsync")]
    public async Task<IActionResult> GetOrganisationAsync([FromRoute] string odsCode)
    {
        var site = await _service.GetOrganisation(odsCode);

        if (site == null)
        {
            return NotFound();
        }
        return Ok(site);
    }

    [HttpGet]
    [Route("{odsCode}/hierarchy")]
    public async Task<IActionResult> GetSiteHierarchyAsync([FromRoute] string odsCode)
    {
        var site = await _service.GetOrganisationHierarchy(odsCode);
        if (site == null)
        {
            return NotFound();
        }
        return Ok(site);
    }

    [HttpGet]
    [Route("hierarchy")]
    public async Task<IActionResult> GetSiteHierarchiesAsync([FromQuery] string odsCodes)
    {
        var site = await _service.GetOrganisationHierarchy(odsCodes.Split(",").ToList());
        if (site == null)
        {
            return NotFound();
        }
        return Ok(site);
    }
}
