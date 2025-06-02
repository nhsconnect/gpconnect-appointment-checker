namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Organisation
{
    public int OrganisationId { get; set; }

    public string OdsCode { get; set; } = null!;

    public short OrganisationTypeId { get; set; }

    public string OrganisationName { get; set; } = null!;

    public string AddressLine1 { get; set; } = null!;

    public string AddressLine2 { get; set; } = null!;

    public string Locality { get; set; } = null!;

    public string City { get; set; } = null!;

    public string County { get; set; } = null!;

    public string Postcode { get; set; } = null!;

    public DateTime AddedDate { get; set; }

    public DateTime LastSyncDate { get; set; }

    public virtual OrganisationType OrganisationType { get; set; } = null!;

    public virtual ICollection<SearchResult> SearchResultConsumerOrganisations { get; set; } = new List<SearchResult>();

    public virtual ICollection<SearchResult> SearchResultProviderOrganisations { get; set; } = new List<SearchResult>();

    public virtual ICollection<Spine> Spines { get; set; } = new List<Spine>();

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
