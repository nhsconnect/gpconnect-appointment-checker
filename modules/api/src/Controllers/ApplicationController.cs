using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ApplicationController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationController(IApplicationService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpPost("synchroniseOrganisation")]
    public async Task<IActionResult> SynchroniseOrganisation([FromBody] DTO.Response.Spine.Organisation request)
    {
        await _service.SynchroniseOrganisation(request);
        return Ok();
    }

    [HttpPost("addSearchGroup")]
    public async Task<IActionResult> AddSearchGroup([FromBody] DTO.Request.Application.SearchGroup request)
    {
        var response = await _service.AddSearchGroup(request);
        return Ok(response);
    }

    [HttpPut("updateSearchGroup/{searchGroupId}")]
    public async Task<ActionResult> UpdateSearchGroup([FromRoute] int searchGroupId)
    {
        await _service.UpdateSearchGroup(searchGroupId);
        return Ok();
    }

    [HttpPost("addSearchResult")]
    public async Task<IActionResult> AddSearchResult([FromBody] DTO.Request.Application.SearchResult request)
    {
        var response = await _service.AddSearchResult(request);
        return Ok(response);
    }

    [HttpGet("searchresult/{searchResultId}", Name = "GetSearchResult")]
    public async Task<IActionResult> GetSearchResult([FromRoute] int searchResultId)
    {
        var response = await _service.GetSearchResult(searchResultId);

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("searchgroup/{searchGroupId}", Name = "GetSearchGroup")]
    public async Task<IActionResult> GetSearchGroup([FromRoute] int searchGroupId)
    {
        var response = await _service.GetSearchGroup(searchGroupId);

        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }

    [HttpGet("searchresultbygroup/{searchGroupId}", Name = "GetSearchResultByGroup")]
    public async Task<IActionResult> GetSearchResultByGroup([FromRoute] int searchGroupId)
    {
        var response = await _service.GetSearchResultByGroup(searchGroupId);
        if (response == null)
        {
            return NotFound();
        }
        return Ok(response);
    }
}
