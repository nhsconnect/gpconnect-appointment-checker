namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class EntryType
{
    public short EntryTypeId { get; set; }

    public string EntryDescription { get; set; } = null!;

    public string? Item1Description { get; set; }

    public string? Item2Description { get; set; }

    public string? Item3Description { get; set; }

    public string? DetailsDescription { get; set; }

    public virtual ICollection<Entry> Entries { get; set; } = new List<Entry>();
}
