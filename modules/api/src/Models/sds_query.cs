using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class sds_query
{
    public string query_name { get; set; } = null!;

    public string search_base { get; set; } = null!;

    public string query_text { get; set; } = null!;

    public string query_attributes { get; set; } = null!;
}
