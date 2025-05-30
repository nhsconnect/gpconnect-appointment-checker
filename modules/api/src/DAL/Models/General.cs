namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class General
{
    public bool SingleRowLock { get; set; }

    public string? ProductName { get; set; }

    public string? ProductVersion { get; set; }

    public short? MaxNumWeeksSearch { get; set; }

    public int LogRetentionDays { get; set; }

    public string GetAccessEmailAddress { get; set; } = null!;

    public short MaxNumberProviderCodesSearch { get; set; }

    public short MaxNumberConsumerCodesSearch { get; set; }

    public int LastAccessHighlightThresholdInDays { get; set; }
}
