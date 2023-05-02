using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public class SecretsManagerConfigProvider : ConfigurationProvider
{
    public IAmazonSecretsManager _client { get; }
    private HashSet<(string, string)> _loadedSsoValues = new();
    private HashSet<(string, string)> _loadedGeneralValues = new();
    private HashSet<(string, string)> _loadedNotificationValues = new();
    private HashSet<(string, string)> _loadedApplicationValues = new();

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
        _loadedSsoValues = await FetchSsoConfigurationAsync("gpcac-secret-end-user-application-sso-configuration", default).ConfigureAwait(false);
        SetData(_loadedSsoValues, triggerReload: false);
        
        _loadedGeneralValues = await FetchGeneralConfigurationAsync("gpcac-secret-shared-general-configuration", default).ConfigureAwait(false);
        SetData(_loadedGeneralValues, triggerReload: false);

        _loadedNotificationValues = await FetchNotificationConfigurationAsync("gpcac-secret-shared-notification-configuration", default).ConfigureAwait(false);
        SetData(_loadedNotificationValues, triggerReload: false);

        _loadedApplicationValues = await FetchApplicationConfigurationAsync("gpcac-secret-end-user-application", default).ConfigureAwait(false);
        SetData(_loadedApplicationValues, triggerReload: false);
    }

    private void SetData(IEnumerable<(string, string)> values, bool triggerReload)
    {
        Data = values.ToDictionary(x => x.Item1, x => x.Item2, StringComparer.InvariantCultureIgnoreCase);
        if (triggerReload)
        {
            OnReload();
        }
    }

    private async Task<HashSet<(string, string)>> FetchSsoConfigurationAsync(string secretName, CancellationToken cancellationToken)
    {
        var secretString = await GetSecretString(secretName, cancellationToken);
        var configuration = PopulateSsoConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchGeneralConfigurationAsync(string secretName, CancellationToken cancellationToken)
    {
        var secretString = await GetSecretString(secretName, cancellationToken);
        var configuration = PopulateGeneralConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchNotificationConfigurationAsync(string secretName, CancellationToken cancellationToken)
    {
        var secretString = await GetSecretString(secretName, cancellationToken);
        var configuration = PopulateNotificationConfiguration(secretString);
        return configuration;
    }

    private async Task<HashSet<(string, string)>> FetchApplicationConfigurationAsync(string secretName, CancellationToken cancellationToken)
    {
        var secretString = await GetSecretString(secretName, cancellationToken);
        var configuration = PopulateApplicationConfiguration(secretString);
        return configuration;
    }

    private static HashSet<(string, string)> PopulateSsoConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<SingleSignOnConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateSsoConfiguration from Secret");
        }

        configuration.Add(("SingleSignOnConfig:ClientId", config.ClientId));
        configuration.Add(("SingleSignOnConfig:ClientSecret", config.ClientSecret));
        configuration.Add(("SingleSignOnConfig:CallbackPath", config.CallbackPath));
        configuration.Add(("SingleSignOnConfig:AuthScheme", config.AuthScheme));
        configuration.Add(("SingleSignOnConfig:ChallengeScheme", config.ChallengeScheme));
        configuration.Add(("SingleSignOnConfig:AuthEndpoint", config.AuthEndpoint));
        configuration.Add(("SingleSignOnConfig:TokenEndpoint", config.TokenEndpoint));
        configuration.Add(("SingleSignOnConfig:MetadataEndpoint", config.MetadataEndpoint));
        configuration.Add(("SingleSignOnConfig:SignedOutCallbackPath", config.SignedOutCallbackPath));
        return configuration;
    }

    private static HashSet<(string, string)> PopulateApplicationConfiguration(string secretString)
    {
        var configuration = new HashSet<(string, string)>();
        var config = JsonConvert.DeserializeObject<ApplicationConfig>(secretString);
        if (config == null)
        {
            throw new Exception("Failed to PopulateSsoConfiguration from Secret");
        }

        configuration.Add(("ApplicationConfig:ApiBaseUrl", config.ApiBaseUrl));
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

        configuration.Add(("NotificationConfig:NewAccountCreatedTemplateId", config.NewAccountCreatedTemplateId));
        configuration.Add(("NotificationConfig:UserDetailsFormTemplateId", config.UserDetailsFormTemplateId));
        configuration.Add(("NotificationConfig:AccountDeactivatedTemplateId", config.AccountDeactivatedTemplateId));
        return configuration;
    }

    private async Task<string> GetSecretString(string secretName, CancellationToken cancellationToken)
    {
        try
        {
            var secretValue = await _client.GetSecretValueAsync(new GetSecretValueRequest { SecretId = secretName }, cancellationToken).ConfigureAwait(false);
            var secretString = secretValue.SecretString;

            if (secretString is null)
            {
                throw new Exception("Failed to Find Secret with populated Data");
            }
            return secretString;
        }
        catch (ResourceNotFoundException e)
        {
            throw new Exception($"Error retrieving secret value (Secret: {secretName})", e);
        }
    }
}
