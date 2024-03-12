namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportFilterRequest
{
    public string? FilterColumn { get; set; } = null;
    public string? FilterValue { get; set; } = null;
    public string? FilterTab { get; set; } = null;
}
