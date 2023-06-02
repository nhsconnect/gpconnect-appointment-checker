using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using IApplicationService = GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces.IApplicationService;
using SearchGroup = GpConnect.AppointmentChecker.Models.SearchGroup;
using SearchResult = GpConnect.AppointmentChecker.Models.SearchResult;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ApplicationService : IApplicationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public ApplicationService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<SearchGroup> GetSearchGroup(int searchGroupId)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/application/searchgroup/{searchGroupId}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SearchGroup>(body, _options);
    }

    public async Task<SearchResult> GetSearchResult(int searchResultId)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/application/searchresult/{searchResultId}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<SearchResult>(body, _options);
    }

    public async Task<List<SearchResultList>> GetSearchResultByGroup(int searchGroupId)
    {
        var response = await _httpClient.GetWithHeadersAsync($"/application/searchresultbygroup/{searchGroupId}", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<SearchResultList>>(body, _options);
    }
}
