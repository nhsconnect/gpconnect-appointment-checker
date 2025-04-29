using gpconnect_appointment_checker.api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;
    private ICacheService _cacheService;

    public UserController(IUserService service, ICacheService cacheService)
    {
        _service = service ?? throw new ArgumentNullException();
        _cacheService = cacheService;
    }

    [HttpGet("organisation/{odsCode}", Name = "GetOrganisation")]
    public async Task<IActionResult> GetOrganisationAsync([FromRoute] string odsCode)
    {
        var site = await _service.GetOrganisation(odsCode);

        if (site == null)
        {
            return NotFound();
        }

        return Ok(site);
    }


    [HttpGet("user-advanced")]
    public async Task<ActionResult> Get([FromQuery] UserListAdvanced userListAdvanced, [FromQuery] int page = 1)
    {
        userListAdvanced.RequestUserId = Convert.ToInt32(Request.Headers[Headers.UserId].ToString());
        var results = await new UserQueryHandler(_service, _cacheService).HandleGetUserRequest(userListAdvanced, page);

            return Ok(results);
    }

    [HttpPost("logonUser")]
    public async Task<ActionResult> LogonUser([FromBody] LogonUser request)
    {
        var user = await _service.LogonUser(request);

        return Ok(user);
    }

    [HttpPost("logoffUser")]
    public async Task<ActionResult> LogoffUser([FromBody] LogoffUser request)
    {
        var user = await _service.LogoffUser(request);

        return Ok(user);
    }

    [HttpPost("addOrUpdateUser")]
    public async Task<ActionResult> AddOrUpdateUser([FromBody] UserCreateAccount userCreateAccount)
    {
        var user = await _service.AddOrUpdateUser(userCreateAccount);

        return Ok(user);
    }

    [HttpPost("addUser")]
    public async Task<ActionResult> AddUser([FromBody] UserAdd userAdd)
    {
        var user = await _service.AddUser(userAdd);

        return Ok(user);
    }

    [HttpGet("emailaddress/{emailAddress}", Name = "GetUserByEmailAddress")]
    public async Task<ActionResult> GetUserByEmailAddress([FromRoute] string emailAddress)
    {
        var user = await _service.GetUser(emailAddress);

        return Ok(user);
    }

    [HttpPut("setuserstatus", Name = "SetUserStatus")]
    public async Task<ActionResult> SetUserStatus([FromBody] UserUpdateStatus userUpdateStatus)
    {
        await _service.SetUserStatus(userUpdateStatus);
        return Ok();
    }

    [HttpPut("setmultisearch", Name = "SetMultiSearch")]
    public async Task<ActionResult> SetMultiSearch([FromBody] UserUpdateMultiSearch userUpdateMultiSearch)
    {
        await _service.SetMultiSearch(userUpdateMultiSearch);
        return Ok();
    }

    [HttpPut("setorgtypesearch", Name = "SetOrgTypeSearch")]
    public async Task<ActionResult> SetOrgTypeSearch([FromBody] UserUpdateOrgTypeSearch userUpdateOrgTypeSearch)
    {
        await _service.SetOrgTypeSearch(userUpdateOrgTypeSearch);
        return Ok();
    }

    [HttpPut("setisadmin", Name = "SetIsAdmin")]
    public async Task<ActionResult> SetIsAdmin([FromBody] UserUpdateIsAdmin userUpdateIsAdmin)
    {
        await _service.SetIsAdmin(userUpdateIsAdmin);
        return Ok();
    }
}