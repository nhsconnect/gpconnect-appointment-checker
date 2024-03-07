using GpConnect.AppointmentChecker.Api.Core;

namespace GpConnect.AppointmentChecker.Api.Helpers;

public static class AttributeExtensions
{
    public static string[] GetInteractionId<T>(this T val) where T : Enum
    {
        return GetAttr<InteractionIdAttribute, T>(val)?.InteractionId;
    }

    private static TAttr GetAttr<TAttr, T>(T val) where TAttr : Attribute
    {
        return (TAttr)typeof(T)
            .GetField(val.ToString())
            ?.GetCustomAttributes(typeof(TAttr), false)
            ?.FirstOrDefault();
    }
}
