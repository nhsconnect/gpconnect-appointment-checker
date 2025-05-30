namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class UserSession
{
    public int UserSessionId { get; set; }

    public int UserId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();

    public virtual ICollection<SearchGroup> SearchGroups { get; set; } = new List<SearchGroup>();

    public virtual ICollection<SpineMessage> SpineMessages { get; set; } = new List<SpineMessage>();

    public virtual User User { get; set; } = null!;

    public virtual ICollection<WebRequest> WebRequests { get; set; } = new List<WebRequest>();
}
