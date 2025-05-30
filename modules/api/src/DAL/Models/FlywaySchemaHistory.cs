namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class FlywaySchemaHistory
{
    public int InstalledRank { get; set; }

    public string? Version { get; set; }

    public string Description { get; set; } = null!;

    public string Type { get; set; } = null!;

    public string Script { get; set; } = null!;

    public int? Checksum { get; set; }

    public string InstalledBy { get; set; } = null!;

    public DateTime InstalledOn { get; set; }

    public int ExecutionTime { get; set; }

    public bool Success { get; set; }
}
