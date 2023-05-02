using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using OrganisationType = GpConnect.AppointmentChecker.Models.OrganisationType;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices;

public class ConfigurationService : IConfigurationService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerSettings _options;

    public ConfigurationService(HttpClient httpClient)
    {
        _httpClient = httpClient;
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
}
