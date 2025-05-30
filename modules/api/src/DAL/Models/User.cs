namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class User
{
    public int UserId { get; set; }

    public string EmailAddress { get; set; } = null!;

    public string DisplayName { get; set; } = null!;

    public int OrganisationId { get; set; }

    public DateTime AddedDate { get; set; }

    public DateTime? AuthorisedDate { get; set; }

    public DateTime? LastLogonDate { get; set; }

    public bool IsAdmin { get; set; }

    public bool MultiSearchEnabled { get; set; }

    public bool? TermsAndConditionsAccepted { get; set; }

    public int? UserAccountStatusId { get; set; }

    public bool OrgTypeSearchEnabled { get; set; }

    public virtual ICollection<Entry> EntryAdminUsers { get; set; } = new List<Entry>();

    public virtual ICollection<Entry> EntryUsers { get; set; } = new List<Entry>();

    public virtual Organisation Organisation { get; set; } = null!;

    public virtual ICollection<SearchExport> SearchExports { get; set; } = new List<SearchExport>();

    public virtual UserAccountStatus? UserAccountStatus { get; set; }

    public virtual ICollection<UserSession> UserSessions { get; set; } = new List<UserSession>();

    public virtual ICollection<WebRequest> WebRequests { get; set; } = new List<WebRequest>();
}
