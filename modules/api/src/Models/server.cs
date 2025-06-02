using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class server
{
    public string id { get; set; } = null!;

    public string? data { get; set; }

    public DateTime lastheartbeat { get; set; }

    public int updatecount { get; set; }
}
