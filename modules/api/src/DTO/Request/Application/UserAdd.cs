using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserAdd
{
    [BindProperty(Name = "email_address", SupportsGet = true)]
    public string EmailAddress { get; set; }
    [BindProperty(Name = "admin_user_id", SupportsGet = true)]
    public int AdminUserId { get; set; }
    [BindProperty(Name = "user_session_id", SupportsGet = true)]
    public int UserSessionId { get; set; }
}
