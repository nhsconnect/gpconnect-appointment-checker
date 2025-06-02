namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class OrganisationType
{
    public short OrganisationTypeId { get; set; }

    public string OrganisationTypeName { get; set; } = null!;

    public virtual ICollection<Organisation> Organisations { get; set; } = new List<Organisation>();
}
