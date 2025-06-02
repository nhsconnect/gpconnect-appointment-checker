using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class entry
{
    public int entry_id { get; set; }

    public int? user_id { get; set; }

    public int? user_session_id { get; set; }

    public short entry_type_id { get; set; }

    public string? item1 { get; set; }

    public string? item2 { get; set; }

    public string? item3 { get; set; }

    public string? details { get; set; }

    public int? entry_elapsed_ms { get; set; }

    public DateTime entry_date { get; set; }

    public int? admin_user_id { get; set; }

    public virtual user? admin_user { get; set; }

    public virtual entry_type entry_type { get; set; } = null!;

    public virtual user? user { get; set; }

    public virtual user_session? user_session { get; set; }
}
