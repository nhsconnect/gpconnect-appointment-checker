using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class _lock
{
    public string resource { get; set; } = null!;

    public int updatecount { get; set; }

    public DateTime? acquired { get; set; }
}
