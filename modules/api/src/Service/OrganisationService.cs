using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace GpConnect.AppointmentChecker.Api.Service;

public class OrganisationService : IOrganisationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _fhirReadClient;
    private readonly JsonSerializerSettings _options;
    private readonly IOptions<OrganisationConfig> _config;

    public OrganisationService(ILogger<NotificationService> logger, IHttpClientFactory httpClientFactory, IOptions<OrganisationConfig> config)
    {
        _config = config;
        _httpClientFactory = httpClientFactory;

        _fhirReadClient = _httpClientFactory.CreateClient(Clients.FHIRREADCLIENT);

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _fhirReadClient.BaseAddress = new Uri(_config.Value.BaseFhirApiUrl + "/");
    }

    public async Task<DTO.Response.Organisation.Organisation> GetOrganisation(string odsCode)
    {
        if (!string.IsNullOrWhiteSpace(odsCode))
        {
            var response = await _fhirReadClient.GetAsync($"{odsCode}");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<DTO.Response.Organisation.Organisation>(body, _options);
            }
        }
        return null;
    }

    public async Task<List<string>> GetOrganisationsFromOdsByRole(string[] roles)
    {
        var bundle = new List<string>();
        var queryStringBuilder = new QueryBuilder
        {
            { "ods-org-role", roles.AsEnumerable().Select(role => role.ToString()) },
            { "_count", _config.Value.RecordLimit.ToString() }
        };
        var page = 1;
        var hasNext = true;

        while (hasNext)
        {
            var response = await _fhirReadClient.GetAsync($"{queryStringBuilder}&_page={page}");
            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var resource = JsonConvert.DeserializeObject<DTO.Response.Fhir.Organisation>(body, _options);
                if (resource != null)
                {
                    bundle.AddRange(from entry in resource.Entries select entry.Resource.OdsCode);
                    hasNext = resource.HasNext;
                }
            }
            page++;
        }
        return bundle;
    }    
}
