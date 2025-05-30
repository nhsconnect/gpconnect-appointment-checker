namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class UserAccountStatus
{
    public int UserAccountStatusId { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
