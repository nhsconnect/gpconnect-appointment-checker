using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class flyway_schema_history
{
    public int installed_rank { get; set; }

    public string? version { get; set; }

    public string description { get; set; } = null!;

    public string type { get; set; } = null!;

    public string script { get; set; } = null!;

    public int? checksum { get; set; }

    public string installed_by { get; set; } = null!;

    public DateTime installed_on { get; set; }

    public int execution_time { get; set; }

    public bool success { get; set; }
}
