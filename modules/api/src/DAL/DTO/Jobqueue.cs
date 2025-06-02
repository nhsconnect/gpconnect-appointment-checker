namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Jobqueue
{
    public long Id { get; set; }

    public long Jobid { get; set; }

    public string Queue { get; set; } = null!;

    public DateTime? Fetchedat { get; set; }

    public int Updatecount { get; set; }
}
