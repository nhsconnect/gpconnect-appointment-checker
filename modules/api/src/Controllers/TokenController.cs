using GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TokenController : ControllerBase
{
    private readonly ITokenService _service;

    public TokenController(ITokenService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet()]
    public async Task<ActionResult> Get([FromBody] RequestParameters request)
    {
        var requestParameters = await _service.ConstructRequestParameters(request);
        return Ok(requestParameters);
    }
}
