using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class SpineService : ISpineService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public SpineService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        _options = new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        };
    }

    public async Task<Spine> GetConsumerDetails(string odsCode)
    {
        var response = await _httpClient.GetAsync($"/spine/consumer/{odsCode}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Spine>(body, _options);
    }

    public async Task<OrganisationSpine> GetOrganisation(string odsCode)
    {
        var response = await _httpClient.GetAsync($"/spine/organisation/{odsCode}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<OrganisationSpine>(body, _options);
    }

    public async Task<Spine> GetProviderDetails(string odsCode)
    {
        var response = await _httpClient.GetAsync($"/spine/provider/{odsCode}");

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<Spine>(body, _options);
    }
}
