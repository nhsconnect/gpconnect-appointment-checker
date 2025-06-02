using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class organisation
{
    public int organisation_id { get; set; }

    public string ods_code { get; set; } = null!;

    public short organisation_type_id { get; set; }

    public string organisation_name { get; set; } = null!;

    public string address_line_1 { get; set; } = null!;

    public string address_line_2 { get; set; } = null!;

    public string locality { get; set; } = null!;

    public string city { get; set; } = null!;

    public string county { get; set; } = null!;

    public string postcode { get; set; } = null!;

    public DateTime added_date { get; set; }

    public DateTime last_sync_date { get; set; }

    public virtual organisation_type organisation_type { get; set; } = null!;

    public virtual ICollection<search_result> search_resultconsumer_organisations { get; set; } = new List<search_result>();

    public virtual ICollection<search_result> search_resultprovider_organisations { get; set; } = new List<search_result>();

    public virtual ICollection<spine> spines { get; set; } = new List<spine>();

    public virtual ICollection<user> users { get; set; } = new List<user>();
}
