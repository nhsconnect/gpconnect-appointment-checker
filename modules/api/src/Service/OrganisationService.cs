using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Fhir;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GpConnect.AppointmentChecker.Api.Service;

public class OrganisationService : IOrganisationService
{
    private readonly ILogger<NotificationService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly HttpClient _fhirReadClient;
    private readonly HttpClient _hierarchyClient;
    private readonly JsonSerializerSettings _options;
    private readonly IOptions<OrganisationConfig> _config;
    private static string _bearerToken;

    public OrganisationService(ILogger<NotificationService> logger, IHttpClientFactory httpClientFactory, IOptions<OrganisationConfig> config)
    {
        _config = config;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory;

        _fhirReadClient = _httpClientFactory.CreateClient(Clients.FHIRREADCLIENT);
        _hierarchyClient = _httpClientFactory.CreateClient(Clients.HIERARCHYCLIENT);

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _fhirReadClient.BaseAddress = new Uri(_config.Value.BaseFhirApiUrl + "/");
        _hierarchyClient.BaseAddress = new Uri(_config.Value.HierarchyOdsApiUrl + "/");
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
                var resource = JsonConvert.DeserializeObject<Organisation>(body, _options);
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

    public async Task<List<Hierarchy>> GetOrganisationHierarchy(List<string> odsCodes)
    {
        _bearerToken = await GetBearerToken();
        _hierarchyClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        var hierarchies = new List<Hierarchy>();

        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 100 };

        await Parallel.ForEachAsync(odsCodes, parallelOptions, async (odsCode, ct) =>
        {
            hierarchies.Add(await GetOrganisationHierarchy(odsCode));
        });
        return hierarchies;
    }

    public async Task<Hierarchy> GetOrganisationHierarchy(string odsCode)
    {
        try
        {
            var organisation = await GetOrganisation(odsCode);
            var hierarchy = new Hierarchy()
            {
                OdsCode = odsCode
            };
            if (organisation != null)
            {
                var postCode = organisation.PostalAddress.PostCode;
                hierarchy.Postcode = postCode;
                hierarchy.SiteName = organisation.OrganisationName;

                const string RE6 = "RE6";
                const string ICB = "ICB";
                const string NHSER = "NHSER";

                var baseCodeSystem = await GetResponse($"{_config.Value.HierarchyFhirBaseUrl}&code={odsCode}&property={RE6}");
                if (baseCodeSystem != null)
                {
                    var partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == RE6) > 0)).ElementAtOrDefault(0);
                    hierarchy.IcbCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueCode;
                    hierarchy.IcbName = (await GetOrganisation(hierarchy.IcbCode))?.OrganisationName;
                }

                baseCodeSystem = await GetResponse($"{_config.Value.HierarchyOdsBaseUrl}&code={postCode}&property={ICB}&property={NHSER}");

                if (baseCodeSystem != null)
                {
                    var partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == ICB) > 0)).ElementAtOrDefault(0);
                    hierarchy.HigherHealthAuthorityCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueString;
                    hierarchy.HigherHealthAuthorityName = (await GetOrganisation(hierarchy.HigherHealthAuthorityCode))?.OrganisationName;

                    partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == NHSER) > 0)).ElementAtOrDefault(0);
                    hierarchy.NationalGroupingCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueString;
                    hierarchy.NationalGroupingName = (await GetOrganisation(hierarchy.NationalGroupingCode))?.OrganisationName;
                }
            }
            return hierarchy;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to obtain the organisation hierarchy");
            throw;
        }
    }

    private async Task<BaseCodeSystem?> GetResponse(string url)
    {
        var response = await _hierarchyClient.GetAsync(url);

        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            var data = JsonConvert.DeserializeObject<BaseCodeSystem>(body, _options);
            return data;
        }
        return null;
    }

    private async Task<string?> GetBearerToken()
    {
        try
        {
            var request = new[]
            {
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
                new KeyValuePair<string, string>("client_id", _config.Value.HierarchyOdsApiClientId),
                new KeyValuePair<string, string>("client_secret", _config.Value.HierarchyOdsApiClientSecret)
            };

            var response = await _hierarchyClient.PostAsync("authorisation/auth/realms/terminology/protocol/openid-connect/token", new FormUrlEncodedContent(request));

            if (response.IsSuccessStatusCode)
            {
                var body = await response.Content.ReadAsStringAsync();
                var token = JsonConvert.DeserializeObject<AccessToken>(body, _options);
                return token?.Token;
            }
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error has occurred while trying to obtain a bearer token");
            throw;
        }
    }
}
