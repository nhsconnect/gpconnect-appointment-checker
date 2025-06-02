using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class web_request
{
    public int web_request_id { get; set; }

    public int? user_id { get; set; }

    public int? user_session_id { get; set; }

    public string url { get; set; } = null!;

    public string? referrer_url { get; set; }

    public string description { get; set; } = null!;

    public string ip { get; set; } = null!;

    public DateTime created_date { get; set; }

    public string created_by { get; set; } = null!;

    public string server { get; set; } = null!;

    public int response_code { get; set; }

    public string session_id { get; set; } = null!;

    public string user_agent { get; set; } = null!;

    public virtual user? user { get; set; }

    public virtual user_session? user_session { get; set; }
}
