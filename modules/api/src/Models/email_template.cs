using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class email_template
{
    public short email_template_id { get; set; }

    public string? subject { get; set; }

    public string? body { get; set; }
}
