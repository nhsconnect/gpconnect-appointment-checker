namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportFilterRequest
{
    public string FilterColumn { get; set; }

    private string _FilterValue;

    public string FilterValue
    {
        get { return _FilterValue.ToUpper(); }
        set { _FilterValue = value; }
    }
}