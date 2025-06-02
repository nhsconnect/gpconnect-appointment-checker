using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class user
{
    public int user_id { get; set; }

    public string email_address { get; set; } = null!;

    public string display_name { get; set; } = null!;

    public int organisation_id { get; set; }

    public DateTime added_date { get; set; }

    public DateTime? authorised_date { get; set; }

    public DateTime? last_logon_date { get; set; }

    public bool is_admin { get; set; }

    public bool multi_search_enabled { get; set; }

    public bool? terms_and_conditions_accepted { get; set; }

    public int? user_account_status_id { get; set; }

    public bool org_type_search_enabled { get; set; }

    public virtual ICollection<entry> entryadmin_users { get; set; } = new List<entry>();

    public virtual ICollection<entry> entryusers { get; set; } = new List<entry>();

    public virtual organisation organisation { get; set; } = null!;

    public virtual ICollection<search_export> search_exports { get; set; } = new List<search_export>();

    public virtual user_account_status? user_account_status { get; set; }

    public virtual ICollection<user_session> user_sessions { get; set; } = new List<user_session>();

    public virtual ICollection<web_request> web_requests { get; set; } = new List<web_request>();
}
