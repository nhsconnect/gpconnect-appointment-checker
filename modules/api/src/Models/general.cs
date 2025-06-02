using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class general
{
    public bool single_row_lock { get; set; }

    public string? product_name { get; set; }

    public string? product_version { get; set; }

    public short? max_num_weeks_search { get; set; }

    public int log_retention_days { get; set; }

    public string get_access_email_address { get; set; } = null!;

    public short max_number_provider_codes_search { get; set; }

    public short max_number_consumer_codes_search { get; set; }

    public int last_access_highlight_threshold_in_days { get; set; }
}
