using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using OrganisationType = GpConnect.AppointmentChecker.Models.OrganisationType;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ConfigurationService : IConfigurationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public ConfigurationService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<List<OrganisationType>> GetOrganisationTypes()
    {
        var response = await _httpClient.GetWithHeadersAsync("/configuration/organisationtypes", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<OrganisationType>>(body, _options);
    }
}