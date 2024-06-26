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

    [HttpGet("ods", Name = "GetOrganisationsFromOdsByRole")]
    public async Task<IActionResult> GetOrganisationsFromOdsByRoleAsync([FromQuery] string roles)
    {
        var sites = await _service.GetOrganisationsFromOdsByRole(roles.Split(","));

        if (sites == null)
        {
            return NotFound();
        }
        return Ok(sites);
    }
}
