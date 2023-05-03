using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using GpConnect.AppointmentChecker.Models.Search;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class SearchService : ISearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public SearchService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
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
