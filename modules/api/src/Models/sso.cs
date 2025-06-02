using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class sso
{
    public bool single_row_lock { get; set; }

    public string client_id { get; set; } = null!;

    public string client_secret { get; set; } = null!;

    public string callback_path { get; set; } = null!;

    public string auth_scheme { get; set; } = null!;

    public string challenge_scheme { get; set; } = null!;

    public string auth_endpoint { get; set; } = null!;

    public string token_endpoint { get; set; } = null!;

    public string metadata_endpoint { get; set; } = null!;

    public string signed_out_callback_path { get; set; } = null!;
}
