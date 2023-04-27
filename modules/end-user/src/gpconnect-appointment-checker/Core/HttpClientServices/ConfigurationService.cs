using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using General = GpConnect.AppointmentChecker.Models.General;
using OrganisationType = GpConnect.AppointmentChecker.Models.OrganisationType;
using Spine = GpConnect.AppointmentChecker.Models.Spine;
using Sso = GpConnect.AppointmentChecker.Models.Sso;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ConfigurationService : IConfigurationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ConfigurationService(HttpClient httpClient, IOptions<ConfigurationServiceConfig> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(options.Value.BaseUrl).Uri;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }    

    public async Task<List<OrganisationType>> GetOrganisationTypes()
    {
        var response = await _httpClient.GetAsync("/configuration/organisationtypes");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<OrganisationType>>(body, _options);
    }

    public async Task<General> GetGeneralConfiguration()
    {
        var response = await _httpClient.GetAsync("/configuration/general");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<General>(body, _options);
    }

    public async Task<Spine> GetSpineConfiguration()
    {
        var response = await _httpClient.GetAsync("/configuration/spine");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Spine>(body, _options);
    }

    public async Task<Sso> GetSsoConfiguration()
    {
        var response = await _httpClient.GetAsync("/configuration/sso");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Sso>(body, _options);
    }
    public async Task<Email> GetEmailConfiguration()
    {
        var response = await _httpClient.GetAsync("/configuration/email");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Email>(body, _options);
    }

    public class ConfigurationServiceConfig
    {
        public string BaseUrl { get; set; } = "";
    }
}
