using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class aggregatedcounter
{
    public long id { get; set; }

    public string key { get; set; } = null!;

    public long value { get; set; }

    public DateTime? expireat { get; set; }
}
