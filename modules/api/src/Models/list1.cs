using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class list1
{
    public long id { get; set; }

    public string key { get; set; } = null!;

    public string? value { get; set; }

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }
}
