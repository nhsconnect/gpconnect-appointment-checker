using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class spine_message_type
{
    public short spine_message_type_id { get; set; }

    public string spine_message_type_name { get; set; } = null!;

    public string? interaction_id { get; set; }

    public virtual ICollection<spine_message> spine_messages { get; set; } = new List<spine_message>();
}
