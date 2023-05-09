using Amazon;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace GpConnect.AppointmentChecker.Core.Configuration;

public class AmazonSecretsManagerConfigurationProvider : ConfigurationProvider
{
    private HashSet<(string, string)> _loadedSsoValues = new();
    private HashSet<(string, string)> _loadedGeneralValues = new();
    private HashSet<(string, string)> _loadedNotificationValues = new();
    private HashSet<(string, string)> _loadedApplicationValues = new();

    public override void Load()
    {
        _loadedSsoValues = FetchSsoConfigurationAsync("gpcac/sso-configuration");
        SetData(_loadedSsoValues, triggerReload: false);

        _loadedGeneralValues = FetchGeneralConfigurationAsync("gpcac/general-configuration");
        SetData(_loadedGeneralValues, triggerReload: false);

        _loadedNotificationValues = FetchNotificationConfigurationAsync("gpcac/notification-configuration");
        SetData(_loadedNotificationValues, triggerReload: false);

        _loadedApplicationValues = FetchApplicationConfigurationAsync("gpcac/enduser-configuration");
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

    private HashSet<(string, string)> FetchSsoConfigurationAsync(string secretName)
    {
        var secretString = GetSecretString(secretName);
        var configuration = PopulateSsoConfiguration(secretString);
        return configuration;
    }

    private HashSet<(string, string)> FetchGeneralConfigurationAsync(string secretName)
    {
        var secretString = GetSecretString(secretName);
        var configuration = PopulateGeneralConfiguration(secretString);
        return configuration;
    }

    private HashSet<(string, string)> FetchNotificationConfigurationAsync(string secretName)
    {
        var secretString = GetSecretString(secretName);
        var configuration = PopulateNotificationConfiguration(secretString);
        return configuration;
    }

    private HashSet<(string, string)> FetchApplicationConfigurationAsync(string secretName)
    {
        var secretString = GetSecretString(secretName);
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

    private string GetSecretString(string secretName)
    {
        var request = new GetSecretValueRequest
        {
            SecretId = secretName
        };

        using (var client = new AmazonSecretsManagerClient(RegionEndpoint.EUWest2))
        {
            var response = client.GetSecretValueAsync(request).Result;

            string secretString;

            if (response.SecretString != null)
            {
                secretString = response.SecretString;
            }
            else
            {
                var memoryStream = response.SecretBinary;
                var reader = new StreamReader(memoryStream);
                secretString = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            }

            return secretString;
        }
    }
}
