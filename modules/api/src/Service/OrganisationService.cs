using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GpConnect.AppointmentChecker.Api.Service;

public class OrganisationService : IOrganisationService
{
    private readonly HttpClient _fhirReadClient;
    private readonly HttpClient _hierarchyClient;
    private readonly JsonSerializerSettings _options;
    private readonly IOptions<OrganisationConfig> _config;

    public OrganisationService(HttpClient fhirReadClient, HttpClient hierarchyClient, IOptions<OrganisationConfig> config)
    {
        _config = config;
        _fhirReadClient = fhirReadClient ?? throw new ArgumentNullException(nameof(fhirReadClient));
        _hierarchyClient = hierarchyClient ?? throw new ArgumentNullException(nameof(hierarchyClient));
        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _fhirReadClient.BaseAddress = new Uri(_config.Value.BaseFhirApiUrl + "/");
        _hierarchyClient.BaseAddress = new Uri(_config.Value.HierarchyOdsApiUrl + "/");
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

    public async Task<Dictionary<string, Hierarchy>> GetOrganisationHierarchy(List<string> odsCodes)
    {
        var hierarchies = new Dictionary<string, Hierarchy>();

        await Parallel.ForEachAsync(odsCodes, async (odsCode, ct) =>
        {
            hierarchies.Add(odsCode, await GetOrganisationHierarchy(odsCode));
        });
        return hierarchies;
    }

    public async Task<Hierarchy> GetOrganisationHierarchy(string odsCode)
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

            var bearerToken = await GetBearerToken();
            _hierarchyClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

            const string RE6 = "RE6";
            const string ICB = "ICB";
            const string NHSER = "NHSER";

            var baseCodeSystem = await GetResponse($"authoring/fhir/CodeSystem/$lookup?system=https://fhir.nhs.uk/Id/ods-organization-code&code={odsCode}&property={RE6}");
            if (baseCodeSystem != null)
            {
                var partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == RE6) > 0)).ElementAtOrDefault(0);
                hierarchy.IcbCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueCode;
                hierarchy.IcbName = (await GetOrganisation(hierarchy.IcbCode))?.OrganisationName;
            }

            baseCodeSystem = await GetResponse($"authoring/fhir/CodeSystem/$lookup?system=https://ods-prototype/postcode&code={postCode}&property={ICB}&property={NHSER}");

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
}
