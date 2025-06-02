namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SearchResult
{
    public int SearchResultId { get; set; }

    public int SearchGroupId { get; set; }

    public string? ConsumerOdsCode { get; set; }

    public int? ConsumerOrganisationId { get; set; }

    public string? ProviderOdsCode { get; set; }

    public int? ProviderOrganisationId { get; set; }

    public int? ErrorCode { get; set; }

    public string? Details { get; set; }

    public string? ProviderPublisher { get; set; }

    public double? SearchDurationSeconds { get; set; }

    public string? ConsumerOrganisationType { get; set; }

    public virtual Organisation? ConsumerOrganisation { get; set; }

    public virtual Organisation? ProviderOrganisation { get; set; }

    public virtual SearchGroup SearchGroup { get; set; } = null!;

    public virtual ICollection<SpineMessage> SpineMessages { get; set; } = new List<SpineMessage>();
}
