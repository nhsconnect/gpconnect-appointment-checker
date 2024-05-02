using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportSource
{
    public string OdsCode { get; set; } = null;
    public string SupplierName { get; set; } = null;
    public Hierarchy Hierarchy { get; set; }
}