using GpConnect.AppointmentChecker.Function.DTO.Response;

namespace GpConnect.AppointmentChecker.Function.DTO.Request;

public class ReportSource
{
    public string OdsCode { get; set; }
    public string SupplierName { get; set; }
    public List<OrganisationHierarchy> OrganisationHierarchies { get; set; }
}
