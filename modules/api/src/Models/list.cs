using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class list
{
    public string? report_name { get; set; }

    public string? function_name { get; set; }

    public string? interaction { get; set; }

    public string? workflow { get; set; }

    public string? report_id { get; set; }
}
