using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class set
{
    public long id { get; set; }

    public string key { get; set; } = null!;

    public double score { get; set; }

    public string value { get; set; } = null!;

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }
}
