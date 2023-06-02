using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserUpdateStatus
{
    public int UserId { get; set; }
    public int UserAccountStatusId { get; set; }
    public int UserSessionId { get; set; }
    public Uri RequestUrl { get; set; }
}
