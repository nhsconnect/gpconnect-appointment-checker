namespace GpConnect.AppointmentChecker.Api.Core;

public class InteractionIdAttribute : Attribute
{
    public string[] InteractionId { get; protected set; }

    public InteractionIdAttribute(params string[] value)
    {
        InteractionId = value;
    }
}
