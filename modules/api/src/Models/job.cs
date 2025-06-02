using System;
using System.Collections.Generic;

namespace gpconnect_appointment_checker.api.Models;

public partial class job
{
    public long id { get; set; }

    public long? stateid { get; set; }

    public string? statename { get; set; }

    public string invocationdata { get; set; } = null!;

    public string arguments { get; set; } = null!;

    public DateTime createdat { get; set; }

    public DateTime? expireat { get; set; }

    public int updatecount { get; set; }

    public virtual ICollection<jobparameter> jobparameters { get; set; } = new List<jobparameter>();

    public virtual ICollection<state> states { get; set; } = new List<state>();
}
