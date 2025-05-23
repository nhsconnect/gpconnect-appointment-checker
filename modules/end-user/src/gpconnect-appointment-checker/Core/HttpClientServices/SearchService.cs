using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;
    private readonly IHttpContextAccessor _contextAccessor;

    public SearchService(HttpClient httpClient, IOptions<ApplicationConfig> config, IHttpContextAccessor contextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;
        _contextAccessor = contextAccessor;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<SearchResultList> ExecuteFreeSlotSearchFromDatabase(SearchRequestFromDatabase searchRequestFromDatabase)
    {
        var query = new Dictionary<string, string?>
            {
                { "search_result_id", searchRequestFromDatabase.SearchResultId.ToString() }
            };

        var request = QueryHelpers.AddQueryString("/search", query);

        var response = await _httpClient.GetWithHeadersAsync(request, new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<SearchResultList>(body, _options);
    }

    public async Task<List<SearchResultList>> ExecuteSearch(SearchRequest searchRequest)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(searchRequest, null, _options),
            Encoding.UTF8,
            MediaTypeNames.Application.Json);

        var response = await _httpClient.PostWithHeadersAsync("/search", new Dictionary<string, string>()
        {
            [Headers.UserId] = _contextAccessor.HttpContext?.User?.GetClaimValue(Headers.UserId)
        }, json);

        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<SearchResultList>>(body, _options);
    }
}
