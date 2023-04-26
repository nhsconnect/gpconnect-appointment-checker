namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class RootResource
{
    public string resourceType { get; set; }
    public string id { get; set; }
    public RootMeta2 meta { get; set; }
    public List<RootIdentifier> identifier { get; set; }
    public object name { get; set; }
    public string gender { get; set; }
    public List<RootTelecom> telecom { get; set; }
    public object address { get; set; }
    public List<RootExtension> extension { get; set; }
    public List<RootServiceType> serviceType { get; set; }
    public RootSchedule schedule { get; set; }
    public string status { get; set; }
    public DateTimeOffset? start { get; set; }
    public DateTimeOffset? end { get; set; }
    public RootServiceCategory serviceCategory { get; set; }
    public List<RootActor> actor { get; set; }
    public RootPlanningHorizon planningHorizon { get; set; }
    public RootManagingOrganization managingOrganization { get; set; }
}