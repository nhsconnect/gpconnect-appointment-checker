using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class entry_type
{
    public short entry_type_id { get; set; }

    public string entry_description { get; set; } = null!;

    public string? item1_description { get; set; }

    public string? item2_description { get; set; }

    public string? item3_description { get; set; }

    public string? details_description { get; set; }

    public virtual ICollection<entry> entries { get; set; } = new List<entry>();
}
