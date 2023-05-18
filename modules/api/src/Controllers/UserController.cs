using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _service;

    public UserController(IUserService service)
    {
        _service = service ?? throw new ArgumentNullException();
    }

    [HttpGet("user-simple")]
    public async Task<ActionResult> Get([FromQuery] UserListSimple userListSimple)
    {
        var users = await _service.GetUsers(userListSimple);
        return Ok(users);
    }

    [HttpGet("user-advanced")]
    public async Task<ActionResult> Get([FromQuery] UserListAdvanced userListAdvanced)
    {
        var users = await _service.GetUsers(userListAdvanced);
        return Ok(users);
    }

    [HttpPost("logonUser")]
    public async Task<ActionResult> LogonUser([FromBody] LogonUser request)
    {
        var user = await _service.LogonUser(request);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("logoffUser")]
    public async Task<ActionResult> LogoffUser([FromBody] LogoffUser request)
    {
        var user = await _service.LogoffUser(request);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("addOrUpdateUser")]
    public async Task<ActionResult> AddOrUpdateUser([FromBody] UserCreateAccount userCreateAccount)
    {
        var user = await _service.AddOrUpdateUser(userCreateAccount);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPost("addUser")]
    public async Task<ActionResult> AddUser([FromBody] UserAdd userAdd)
    {
        var user = await _service.AddUser(userAdd);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpGet("emailaddress/{emailAddress}", Name = "GetUserByEmailAddress")]
    public async Task<ActionResult> GetUserByEmailAddress([FromRoute] string emailAddress)
    {
        var user = await _service.GetUser(emailAddress);

        if (user == null)
        {
            return NotFound();
        }

        return Ok(user);
    }

    [HttpPut("setuserstatus", Name = "SetUserStatus")]
    public async Task<ActionResult> SetUserStatus([FromBody] UserUpdateStatus userUpdateStatus)
    {
        var user = await _service.SetUserStatus(userUpdateStatus);
        return Ok(user);
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
}
