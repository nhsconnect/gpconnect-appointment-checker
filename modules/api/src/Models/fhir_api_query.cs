using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class fhir_api_query
{
    public string query_name { get; set; } = null!;

    public string query_text { get; set; } = null!;
}
