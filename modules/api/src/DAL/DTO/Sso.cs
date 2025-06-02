namespace gpconnect_appointment_checker.api.DAL.Models;

public partial class Sso
{
    public bool SingleRowLock { get; set; }

    public string ClientId { get; set; } = null!;

    public string ClientSecret { get; set; } = null!;

    public string CallbackPath { get; set; } = null!;

    public string AuthScheme { get; set; } = null!;

    public string ChallengeScheme { get; set; } = null!;

    public string AuthEndpoint { get; set; } = null!;

    public string TokenEndpoint { get; set; } = null!;

    public string MetadataEndpoint { get; set; } = null!;

    public string SignedOutCallbackPath { get; set; } = null!;
}
