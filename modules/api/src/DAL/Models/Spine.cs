namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Spine
{
    public bool SingleRowLock { get; set; }

    public bool UseSsp { get; set; }

    public string? SspHostname { get; set; }

    public string SdsHostname { get; set; } = null!;

    public int SdsPort { get; set; }

    public bool SdsUseLdaps { get; set; }

    public int OrganisationId { get; set; }

    public string PartyKey { get; set; } = null!;

    public string Asid { get; set; } = null!;

    public int TimeoutSeconds { get; set; }

    public string? ClientCert { get; set; }

    public string? ClientPrivateKey { get; set; }

    public string? ServerCaCertchain { get; set; }

    public bool SdsUseMutualauth { get; set; }

    public string SpineFqdn { get; set; } = null!;

    public string? SdsTlsVersion { get; set; }

    public bool SdsUseFhirApi { get; set; }

    public string SpineFhirApiDirectoryServicesFqdn { get; set; } = null!;

    public string SpineFhirApiSystemsRegisterFqdn { get; set; } = null!;

    public string? SpineFhirApiKey { get; set; }

    public virtual Organisation Organisation { get; set; } = null!;
}
