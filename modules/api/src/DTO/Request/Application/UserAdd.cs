﻿namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserAdd
{
    public string EmailAddress { get; set; }
    public int AdminUserId { get; set; }
    public int UserSessionId { get; set; }
}
