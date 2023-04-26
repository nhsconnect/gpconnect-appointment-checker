using GpConnect.AppointmentChecker.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IApplicationService = GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces.IApplicationService;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ApplicationService : IApplicationService
{
    private readonly ILogger<ApplicationService> _logger;
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ApplicationService(ILogger<ApplicationService> logger, HttpClient httpClient, IOptions<ApplicationServiceConfig> options)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new UriBuilder(options.Value.BaseUrl).Uri;

        _logger = logger;
        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<Organisation> GetOrganisationAsync(string odsCode)
    {
        var response = await _httpClient.GetAsync($"/application/{odsCode}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Organisation>(body, _options);
    }

    public async Task<Models.SearchExport> GetSearchExport(int searchExportId, int userId)
    {
        var response = await _httpClient.GetAsync($"/application/searchexport/{searchExportId}/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Models.SearchExport>(body, _options);
    }

    public async Task<Models.SearchGroup> GetSearchGroup(int searchGroupId, int userId)
    {
        var response = await _httpClient.GetAsync($"/application/searchexport/{searchGroupId}/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Models.SearchGroup>(body, _options);
    }

    public async Task<SearchGroupExport> GetSearchGroupExport(int searchGroupId, int userId)
    {
        var response = await _httpClient.GetAsync($"/application/searchgroupexport/{searchGroupId}/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<SearchGroupExport>(body, _options);
    }

    public async Task<Models.SearchResult> GetSearchResult(int searchResultId, int userId)
    {
        var response = await _httpClient.GetAsync($"/application/searchresult/{searchResultId}/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Models.SearchResult>(body, _options);
    }

    public async Task<List<SlotEntrySummary>> GetSearchResultByGroup(int searchGroupId, int userId)
    {
        var response = await _httpClient.GetAsync($"/application/searchresultbygroup/{searchGroupId}/{userId}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<SlotEntrySummary>>(body, _options);
    }

    public async Task<Models.SearchExport> AddSearchExport(Models.Request.SearchExport request)
    {
        var json = new StringContent(JsonConvert.SerializeObject(request, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/application/addSearchExport", json);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Models.SearchExport>(content, _options);
        return result;
    }

    public async Task<Models.SearchGroup> AddSearchGroup(Models.Request.SearchGroup request)
    {
        var json = new StringContent(JsonConvert.SerializeObject(request, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/application/addSearchGroup", json);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Models.SearchGroup>(content, _options);
        return result;
    }

    public async Task<Models.SearchResult> AddSearchResult(Models.Request.SearchResult request)
    {
        var json = new StringContent(JsonConvert.SerializeObject(request, null, _options),
            Encoding.UTF8,
            MediaTypeHeaderValue.Parse("application/json").MediaType);

        var response = await _httpClient.PostAsync("/application/addSearchResult", json);

        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();

        var result = JsonConvert.DeserializeObject<Models.SearchResult>(content, _options);
        return result;
    }

    public async Task UpdateSearchGroup(int searchGroupId, int userId)
    {
        var response = await _httpClient.PutAsync($"/application/updateSearchGroup/{searchGroupId}/{userId}", null);
        response.EnsureSuccessStatusCode();
    }

    public class ApplicationServiceConfig
    {
        public string BaseUrl { get; set; } = "";
    }
}
