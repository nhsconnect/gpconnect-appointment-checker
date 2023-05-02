using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.Service;

public class OrganisationService : IOrganisationService
{
    private readonly HttpClient _fhirReadClient;
    private readonly HttpClient _odsClient;
    private readonly JsonSerializerSettings _options;
    private readonly IOptions<OrganisationConfig> _config;

    public OrganisationService(HttpClient fhirReadClient, HttpClient odsClient, IOptions<OrganisationConfig> config)
    {
        _config = config;
        _fhirReadClient = fhirReadClient ?? throw new ArgumentNullException(nameof(fhirReadClient));
        _odsClient = odsClient ?? throw new ArgumentNullException(nameof(odsClient));
        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _fhirReadClient.BaseAddress = new Uri(_config.Value.BaseFhirApiUrl + "/");
        _odsClient.BaseAddress = new Uri(_config.Value.BaseOdsApiUrl);
    }

    public async Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode)
    {
        var response = await _fhirReadClient.GetAsync($"{odsCode}");
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<DTO.Response.Organisation.Organisation>(body, _options);
        }
        return null;
    }    
}
