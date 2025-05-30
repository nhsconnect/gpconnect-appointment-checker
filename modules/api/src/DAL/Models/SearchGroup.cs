namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SearchGroup
{
    public int SearchGroupId { get; set; }

    public int UserSessionId { get; set; }

    public string? ConsumerOdsTextbox { get; set; }

    public string ProviderOdsTextbox { get; set; } = null!;

    public string SelectedDaterange { get; set; } = null!;

    public DateTime SearchStartAt { get; set; }

    public DateTime? SearchEndAt { get; set; }

    public string? ConsumerOrganisationTypeDropdown { get; set; }

    public virtual ICollection<SearchResult> SearchResults { get; set; } = new List<SearchResult>();

    public virtual UserSession UserSession { get; set; } = null!;
}
