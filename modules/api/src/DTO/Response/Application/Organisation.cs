namespace GpConnect.AppointmentChecker.Api.DTO.Response.Application;

public class Organisation
{
    public int OrganisationId { get; set; }
    public string ODSCode { get; set; }
    public string OrganisationTypeName { get; set; }
    public string OrganisationName { get; set; }
    public string AddressLine1 { get; set; }
    public string AddressLine2 { get; set; }
    public string Locality { get; set; }
    public string City { get; set; }
    public string County { get; set; }
    public string Postcode { get; set; }
}
