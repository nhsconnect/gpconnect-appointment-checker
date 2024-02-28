using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SpineController : ControllerBase
{
    private readonly ISpineService _service;

    public SpineController(ISpineService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("organisation/{odsCode}", Name = "GetOrganisationDetailsByOdsCodeAsync")]
    public async Task<IActionResult> GetOrganisationDetailsByOdsCodeAsync([FromRoute] string odsCode)
    {
        var site = await _service.GetOrganisationDetailsByOdsCodeAsync(odsCode);

        if (site == null)
        {
            return NotFound();
        }
        return Ok(site);
    }

    [HttpGet("provider/{odsCode}/{interactionId?}", Name = "GetProviderDetails")]
    public async Task<IActionResult> GetProviderDetails([FromRoute] string odsCode, string? interactionId = null)
    {
        var provider = await _service.GetProviderDetails(odsCode, interactionId);

        if (provider == null)
        {
            return NotFound();
        }
        return Ok(provider);
    }

    [HttpGet("consumer/{odsCode}", Name = "GetConsumerDetails")]
    public async Task<IActionResult> GetConsumerDetails([FromRoute] string odsCode)
    {
        var consumer = await _service.GetConsumerDetails(odsCode);

        if (consumer == null)
        {
            return NotFound();
        }
        return Ok(consumer);
    }
}