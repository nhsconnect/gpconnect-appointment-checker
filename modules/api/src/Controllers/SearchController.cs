using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _service;

    public SearchController(ISearchService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpPost()]
    public async Task<ActionResult> ExecuteSearch([FromBody] SearchRequest searchRequest)
    {
        if (!searchRequest.ValidSearchCombination)
        {
            return BadRequest();
        }

        try
        {
            var result = await _service.ExecuteSearch(searchRequest);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet()]
    public async Task<ActionResult> ExecuteFreeSlotSearchFromDatabase(
        [FromQuery] SearchFromDatabaseRequest searchFromDatabaseRequest)
    {
        var result = await _service.ExecuteFreeSlotSearchFromDatabase(searchFromDatabaseRequest);
        if (result == null)
        {
            return NotFound();
        }

        return Ok(result);
    }
}