using System.Collections.Generic;

namespace GpConnect.AppointmentChecker.Models.Request;

public class ReportExport
{
    public List<string>? OdsCodes { get; set; } = null;
    public string? InteractionId { get; set; } = null;
    public string? FunctionName { get; set; } = null;
    public string? ReportName { get; set; } = null;
}
