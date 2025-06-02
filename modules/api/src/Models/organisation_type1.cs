using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class organisation_type1
{
    public short organisation_type_id { get; set; }

    public string organisation_type_code { get; set; } = null!;

    public string organisation_type_description { get; set; } = null!;
}
