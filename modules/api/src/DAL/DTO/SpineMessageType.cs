namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class SpineMessageType
{
    public short SpineMessageTypeId { get; set; }

    public string SpineMessageTypeName { get; set; } = null!;

    public string? InteractionId { get; set; }

    public virtual ICollection<SpineMessage> SpineMessages { get; set; } = new List<SpineMessage>();
}
