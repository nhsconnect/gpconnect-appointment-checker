using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class error_log
{
    public int id { get; set; }

    public string? application { get; set; }

    public DateTime logged { get; set; }

    public string? level { get; set; }

    public string? message { get; set; }

    public string? logger { get; set; }

    public string? callsite { get; set; }

    public string? exception { get; set; }

    public int? user_id { get; set; }

    public int? user_session_id { get; set; }
}
