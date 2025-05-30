namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SdsQuery
{
    public string QueryName { get; set; } = null!;

    public string SearchBase { get; set; } = null!;

    public string QueryText { get; set; } = null!;

    public string QueryAttributes { get; set; } = null!;
}
