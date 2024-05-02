using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

namespace GpConnect.AppointmentChecker.Api.DTO.Request;

public class ReportSource : DataSource
{
    public Hierarchy? OrganisationHierarchy { get; set; }
}