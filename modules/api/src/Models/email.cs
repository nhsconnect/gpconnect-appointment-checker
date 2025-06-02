using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class email
{
    public bool single_row_lock { get; set; }

    public string sender_address { get; set; } = null!;

    public string host_name { get; set; } = null!;

    public short port { get; set; }

    public string encryption { get; set; } = null!;

    public string user_name { get; set; } = null!;

    public string password { get; set; } = null!;

    public string default_subject { get; set; } = null!;
}
