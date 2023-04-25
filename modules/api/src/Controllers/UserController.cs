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
    public async Task<ActionResult> AddUser([FromQuery] UserAdd userAdd)
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

    [HttpPut("setuserstatus/{userId}/{userAccountStatusId}/{adminUserId}/{userSessionId}", Name = "SetUserStatus")]
    public async Task<ActionResult> SetUserStatus(int userId, int userAccountStatusId, int adminUserId, int userSessionId)
    {
        var user = await _service.SetUserStatus(userId, userAccountStatusId, adminUserId, userSessionId);
        return Ok(user);
    }

    [HttpPut("setmultisearch/{userId}/{multiSearchEnabled}/{adminUserId}/{userSessionId}", Name = "SetMultiSearch")]
    public async Task<ActionResult> SetMultiSearch(int userId, bool multiSearchEnabled, int adminUserId, int userSessionId)
    {
        await _service.SetMultiSearch(userId, multiSearchEnabled, adminUserId, userSessionId);
        return Ok();
    }

    [HttpPut("setorgtypesearch/{userId}/{orgTypeSearchEnabled}/{adminUserId}/{userSessionId}", Name = "SetOrgTypeSearch")]
    public async Task<ActionResult> SetOrgTypeSearch(int userId, bool orgTypeSearchEnabled, int adminUserId, int userSessionId)
    {
        await _service.SetOrgTypeSearch(userId, orgTypeSearchEnabled, adminUserId, userSessionId);
        return Ok();
    }
}
