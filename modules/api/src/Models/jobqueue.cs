using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class jobqueue
{
    public long id { get; set; }

    public long jobid { get; set; }

    public string queue { get; set; } = null!;

    public DateTime? fetchedat { get; set; }

    public int updatecount { get; set; }
}
