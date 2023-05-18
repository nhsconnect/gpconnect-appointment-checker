using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class AddUser
{
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
    public string EmailAddress { get; set; }
    public Uri RequestUrl { get; set; }
}
