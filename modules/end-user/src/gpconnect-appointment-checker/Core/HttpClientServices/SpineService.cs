using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class SpineService : ISpineService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public SpineService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<Spine> GetConsumerDetails(string odsCode)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/spine/consumer/{odsCode}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Spine>(body, _options);
    }

    public async Task<OrganisationSpine> GetOrganisation(string odsCode)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/spine/organisation/{odsCode}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<OrganisationSpine>(body, _options);
    }

    public async Task<Spine> GetProviderDetails(string odsCode)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/spine/provider/{odsCode}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<Spine>(body, _options);
    }
}
