namespace GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;

public class OrganisationRelationship
{
    public Organisation Organisation { get; set; }
}

public class Organisation
{
    public string Name { get; set; }
    public Rels Rels { get; set; }
}

public class Rels
{
    public List<Rel> Rel { get; set; }
}

public class Rel
{
    public string Status { get; set; }
    public Target Target { get; set; }
    public string id { get; set; }
}

public class Target
{
    public OrgId OrgId { get; set; }
    public PrimaryRoleId PrimaryRoleId { get; set; }
}

public class OrgId
{
    public string extension { get; set; }
}

public class PrimaryRoleId
{
    public string id { get; set; }
}
