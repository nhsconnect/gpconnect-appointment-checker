using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class search_group
{
    public int search_group_id { get; set; }

    public int user_session_id { get; set; }

    public string? consumer_ods_textbox { get; set; }

    public string provider_ods_textbox { get; set; } = null!;

    public string selected_daterange { get; set; } = null!;

    public DateTime search_start_at { get; set; }

    public DateTime? search_end_at { get; set; }

    public string? consumer_organisation_type_dropdown { get; set; }

    public virtual ICollection<search_result> search_results { get; set; } = new List<search_result>();

    public virtual user_session user_session { get; set; } = null!;
}
