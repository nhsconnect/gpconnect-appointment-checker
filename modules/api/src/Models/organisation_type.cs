using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class organisation_type
{
    public short organisation_type_id { get; set; }

    public string organisation_type_name { get; set; } = null!;

    public virtual ICollection<organisation> organisations { get; set; } = new List<organisation>();
}
