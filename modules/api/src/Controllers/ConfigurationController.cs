using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ConfigurationController : ControllerBase
{
    private readonly IConfigurationService _service;

    public ConfigurationController(IConfigurationService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("spinemessagetypes", Name = "GetSpineMessageTypes")]
    public async Task<IActionResult> GetSpineMessageTypes()
    {
        var response = await _service.GetSpineMessageTypes();

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("organisationtypes", Name = "GetOrganisationTypes")]
    public async Task<IActionResult> GetOrganisationTypes()
    {
        var response = await _service.GetOrganisationTypes();

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("organisationtype/{organisationTypeCode}", Name = "GetOrganisationType")]
    public async Task<IActionResult> GetOrganisationType(string organisationTypeCode)
    {
        var response = await _service.GetOrganisationType(organisationTypeCode);

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("sdsqueryconfiguration/{queryName}", Name = "GetSdsQueryConfiguration")]
    public async Task<IActionResult> GetSdsQueryConfiguration(string queryName)
    {
        var response = await _service.GetSdsQueryConfiguration(queryName);

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("fhirapiqueryconfiguration/{queryName}", Name = "GetFhirApiQueryConfiguration")]
    public async Task<IActionResult> GetFhirApiQueryConfiguration(string queryName)
    {
        var response = await _service.GetFhirApiQueryConfiguration(queryName);

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("sdsqueryconfiguration", Name = "GetSdsQueryConfigurations")]
    public async Task<IActionResult> GetSdsQueryConfigurations()
    {
        var response = await _service.GetSdsQueryConfiguration();

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("fhirapiqueryconfiguration", Name = "GetFhirApiQueryConfigurations")]
    public async Task<IActionResult> GetFhirApiQueryConfigurations()
    {
        var response = await _service.GetFhirApiQueryConfiguration();

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }    
}