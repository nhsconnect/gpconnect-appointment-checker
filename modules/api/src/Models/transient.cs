using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class transient
{
    public string transient_id { get; set; } = null!;

    public string transient_data { get; set; } = null!;

    public string transient_report_id { get; set; } = null!;

    public string transient_report_name { get; set; } = null!;

    public DateTime entry_date { get; set; }
}
