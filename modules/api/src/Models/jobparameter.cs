using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class jobparameter
{
    public long id { get; set; }

    public long jobid { get; set; }

    public string name { get; set; } = null!;

    public string? value { get; set; }

    public int updatecount { get; set; }

    public virtual job job { get; set; } = null!;
}
