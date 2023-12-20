using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserUpdateIsAdmin
{
    public int UserId { get; set; }
    public bool IsAdmin { get; set; }
    public int UserSessionId { get; set; }
    public Uri RequestUrl { get; set; }
}
