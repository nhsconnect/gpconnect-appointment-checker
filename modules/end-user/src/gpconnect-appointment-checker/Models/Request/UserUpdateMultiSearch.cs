using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserUpdateMultiSearch
{
    public int UserId { get; set; }
    public bool MultiSearchEnabled { get; set; }
    public int UserSessionId { get; set; }
    public Uri RequestUrl { get; set; }
}
