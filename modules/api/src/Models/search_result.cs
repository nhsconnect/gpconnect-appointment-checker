using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class search_result
{
    public int search_result_id { get; set; }

    public int search_group_id { get; set; }

    public string? consumer_ods_code { get; set; }

    public int? consumer_organisation_id { get; set; }

    public string? provider_ods_code { get; set; }

    public int? provider_organisation_id { get; set; }

    public int? error_code { get; set; }

    public string? details { get; set; }

    public string? provider_publisher { get; set; }

    public double? search_duration_seconds { get; set; }

    public string? consumer_organisation_type { get; set; }

    public virtual organisation? consumer_organisation { get; set; }

    public virtual organisation? provider_organisation { get; set; }

    public virtual search_group search_group { get; set; } = null!;

    public virtual ICollection<spine_message> spine_messages { get; set; } = new List<spine_message>();
}
