﻿namespace GpConnect.AppointmentChecker.Api.DTO.Request.GpConnect;

public class RequestingDevice : BaseRequest
{
    public string model { get; set; }
    public string version { get; set; }
}
