using gpconnect_appointment_checker.api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class Cache : ControllerBase
{
    private readonly ICacheService _cacheService;
    private readonly IUserService _userService;

    public Cache(ICacheService cacheService, IUserService service)
    {
        _cacheService = cacheService ?? throw new ArgumentNullException();
        _userService = service ?? throw new ArgumentNullException();
    }

    [HttpGet]
    public async Task<IActionResult> CacheTest()
    {
        return Ok();
    }
}