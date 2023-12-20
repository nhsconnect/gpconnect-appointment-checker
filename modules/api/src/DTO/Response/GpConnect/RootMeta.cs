using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class RootMeta
{
    public DateTime lastUpdated { get; set; }
    public List<string> profile { get; set; }
}