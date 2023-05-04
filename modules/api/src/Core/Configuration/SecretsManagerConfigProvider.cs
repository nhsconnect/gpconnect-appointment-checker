using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.Core.Configuration;

public class SecretsManagerConfigProvider : ConfigurationProvider
{
    public IAmazonSecretsManager _client { get; }
    private HashSet<(string, string)> _loadedSpineValues = new();
    private HashSet<(string, string)> _loadedGeneralValues = new();
    private HashSet<(string, string)> _loadedNotificationValues = new();
    private HashSet<(string, string)> _loadedOrganisationValues = new();

    public SecretsManagerConfigProvider(IAmazonSecretsManager client)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public override void Load()
    {
        LoadAsync().ConfigureAwait(false).GetAwaiter().GetResult();
    }

    private async Task LoadAsync()
    {
        _loadedGeneralValues = await FetchGeneralConfigurationAsync("gpcac/general-configuration");
        SetData(_loadedGeneralValues, triggerReload: false);

        _loadedNotificationValues = await FetchNotificationConfigurationAsync("gpcac/notification-configuration");
        SetData(_loadedNotificationValues, triggerReload: false);

        _loadedSpineValues = await FetchSpineConfigurationAsync("gpcac/spine-configuration");
        SetData(_loadedSpineValues, triggerReload: false);

        _loadedOrganisationValues = await FetchOrganisationConfigurationAsync("gpcac/organisation-configuration");
        SetData(_loadedOrganisationValues, triggerReload: false);
    }

    private void SetData(IEnumerable<(string, string)> values, bool triggerReload)
    {
        Data = values.ToDictionary(x => x.Item1, x => x.Item2, StringComparer.InvariantCultureIgnoreCase);
        if (triggerReload)
        {
            OnReload();
        }
    }

    private async Task<HashSet<(string, string)>> FetchSpineConfigurationAsync(string secretName)
    {
        var secretString = await GetSecretString(secretName);
        var configuration = PopulateSpineConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchGeneralConfigurationAsync(string secretName)
    {
        var secretString = await GetSecretString(secretName);
        var configuration = PopulateGeneralConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchNotificationConfigurationAsync(string secretName)
    {
        var secretString = await GetSecretString(secretName);
        var configuration = PopulateNotificationConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchOrganisationConfigurationAsync(string secretName)
    {
        var secretString = await GetSecretString(secretName);
        var configuration = PopulateOrganisationConfiguration(secretString);
        return configuration;
    }

    private static HashSet<(string, string)> PopulateSpineConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<SpineConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateSpineConfiguration from Secret");
        }

        configuration.Add(("SpineConfig:UseSSP", config.UseSSP.ToString()));
        configuration.Add(("SpineConfig:SspHostname", config.SspHostname));
        configuration.Add(("SpineConfig:SdsHostname", config.SdsHostname));
        configuration.Add(("SpineConfig:SdsPort", config.SdsPort.ToString()));
        configuration.Add(("SpineConfig:OrganisationId", config.OrganisationId.ToString()));
        configuration.Add(("SpineConfig:OdsCode", config.OdsCode));
        configuration.Add(("SpineConfig:OrganisationName", config.OrganisationName));
        configuration.Add(("SpineConfig:PartyKey", config.PartyKey));
        configuration.Add(("SpineConfig:AsId", config.AsId));
        configuration.Add(("SpineConfig:TimeoutSeconds", config.TimeoutSeconds.ToString()));
        configuration.Add(("SpineConfig:ClientCert", config.ClientCert));
        configuration.Add(("SpineConfig:ClientPrivateKey", config.ClientPrivateKey));
        configuration.Add(("SpineConfig:ServerCACertChain", config.ServerCACertChain));
        configuration.Add(("SpineConfig:SdsUseMutualAuth", config.SdsUseMutualAuth.ToString()));
        configuration.Add(("SpineConfig:SpineFqdn", config.SpineFqdn));
        configuration.Add(("SpineConfig:SdsTlsVersion", config.SdsTlsVersion));
        configuration.Add(("SpineConfig:SdsUseFhirApi", config.SdsUseFhirApi.ToString()));
        configuration.Add(("SpineConfig:SpineFhirApiDirectoryServicesFqdn", config.SpineFhirApiDirectoryServicesFqdn));
        configuration.Add(("SpineConfig:SpineFhirApiSystemsRegisterFqdn", config.SpineFhirApiSystemsRegisterFqdn));
        configuration.Add(("SpineConfig:SpineFhirApiKey", config.SpineFhirApiKey));
        return configuration;
    }

    private static HashSet<(string, string)> PopulateGeneralConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<GeneralConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateGeneralConfiguration from Secret");
        }

        configuration.Add(("GeneralConfig:ProductName", config.ProductName));
        configuration.Add(("GeneralConfig:ProductVersion", config.ProductVersion));
        configuration.Add(("GeneralConfig:MaxNumWeeksSearch", config.MaxNumWeeksSearch.ToString()));
        configuration.Add(("GeneralConfig:LogRetentionDays", config.LogRetentionDays.ToString()));
        configuration.Add(("GeneralConfig:GetAccessEmailAddress", config.GetAccessEmailAddress));
        configuration.Add(("GeneralConfig:MaxNumberProviderCodesSearch", config.MaxNumberProviderCodesSearch.ToString()));
        configuration.Add(("GeneralConfig:MaxNumberConsumerCodesSearch", config.MaxNumberConsumerCodesSearch.ToString()));
        return configuration;
    }

    private static HashSet<(string, string)> PopulateNotificationConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<NotificationConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateNotificationConfiguration from Secret");
        }

        configuration.Add(("NotificationConfig:ApiKey", config.ApiKey));
        return configuration;
    }

    private static HashSet<(string, string)> PopulateOrganisationConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<OrganisationConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateOrganisationConfiguration from Secret");
        }

        configuration.Add(("OrganisationConfig:BaseFhirApiUrl", config.BaseFhirApiUrl));
        configuration.Add(("OrganisationConfig:BaseOdsApiUrl", config.BaseOdsApiUrl));
        return configuration;
    }

    private async Task<string> GetSecretString(string secretName)
    {
        var secretValueRequest = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        GetSecretValueResponse response;

        try
        {
            response = await _client.GetSecretValueAsync(secretValueRequest);
        }
        catch
        {
            throw;
        }
        return response.SecretString;
    }
}
