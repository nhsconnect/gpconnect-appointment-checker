using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class search_export
{
    public int search_export_id { get; set; }

    public int user_id { get; set; }

    public string search_export_data { get; set; } = null!;

    public DateTime created_date { get; set; }

    public virtual user user { get; set; } = null!;
}
