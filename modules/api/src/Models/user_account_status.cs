using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class user_account_status
{
    public int user_account_status_id { get; set; }

    public string description { get; set; } = null!;

    public virtual ICollection<user> users { get; set; } = new List<user>();
}
