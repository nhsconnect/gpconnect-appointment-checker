using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class spine_message
{
    public int spine_message_id { get; set; }

    public short spine_message_type_id { get; set; }

    public int? user_session_id { get; set; }

    public string? command { get; set; }

    public string? request_headers { get; set; }

    public string request_payload { get; set; } = null!;

    public string? response_status { get; set; }

    public string? response_headers { get; set; }

    public string response_payload { get; set; } = null!;

    public DateTime logged_date { get; set; }

    public double roundtriptime_ms { get; set; }

    public int? search_result_id { get; set; }

    public virtual search_result? search_result { get; set; }

    public virtual spine_message_type spine_message_type { get; set; } = null!;

    public virtual user_session? user_session { get; set; }
}
