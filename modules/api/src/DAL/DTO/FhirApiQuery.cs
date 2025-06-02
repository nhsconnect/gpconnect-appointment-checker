namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class FhirApiQuery
{
    public string QueryName { get; set; } = null!;

    public string QueryText { get; set; } = null!;
}
