using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class user_session
{
    public int user_session_id { get; set; }

    public int user_id { get; set; }

    public DateTime start_time { get; set; }

    public DateTime? end_time { get; set; }

    public virtual ICollection<entry> entries { get; set; } = new List<entry>();

    public virtual ICollection<search_group> search_groups { get; set; } = new List<search_group>();

    public virtual ICollection<spine_message> spine_messages { get; set; } = new List<spine_message>();

    public virtual user user { get; set; } = null!;

    public virtual ICollection<web_request> web_requests { get; set; } = new List<web_request>();
}
