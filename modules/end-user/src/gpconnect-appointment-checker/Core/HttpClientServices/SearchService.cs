using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Request;
using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public SearchService(HttpClient httpClient, IOptions<ApplicationConfig> config)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(config.Value.ApiBaseUrl).Uri;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<SearchResultList> ExecuteFreeSlotSearchFromDatabase(SearchRequestFromDatabase searchRequestFromDatabase)
    {
        var query = new Dictionary<string, string?>
            {
                { "search_result_id", searchRequestFromDatabase.SearchResultId.ToString() },
                { "user_id", searchRequestFromDatabase.UserId.ToString() }
            };

        var request = QueryHelpers.AddQueryString("/search", query);

        var response = await _httpClient.GetAsync(request, HttpCompletionOption.ResponseHeadersRead);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<SearchResultList>(content, _options);
        return result;
    }

    public async Task<List<SearchResultList>> ExecuteSearch(SearchRequest searchRequest)
    {
        var json = new StringContent(
            JsonConvert.SerializeObject(searchRequest, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/search", json);
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<List<SearchResultList>>(content, _options);
        return result;
    }
}
