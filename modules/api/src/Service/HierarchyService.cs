using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Dal.Enumerations;
using GpConnect.AppointmentChecker.Api.DTO.Response.Organisation.Hierarchy;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace GpConnect.AppointmentChecker.Api.Service;

public class HierarchyService : IHierarchyService
{
    private readonly ILogger<HierarchyService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOrganisationService _organisationService;
    private readonly HttpClient _hierarchyClient;
    private readonly HttpClient _odsClient;
    private readonly JsonSerializerSettings _options;
    private readonly IOptions<HierarchyConfig> _config;
    private static string _bearerToken;

    public HierarchyService(ILogger<HierarchyService> logger, IHttpClientFactory httpClientFactory, IOptions<HierarchyConfig> config, IOrganisationService organisationService)
    {
        _config = config;
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory;
        _organisationService = organisationService ?? throw new ArgumentNullException(nameof(organisationService));

        _hierarchyClient = _httpClientFactory.CreateClient(Clients.HIERARCHYCLIENT);
        _odsClient = _httpClientFactory.CreateClient(Clients.ODSCLIENT);

        _options = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore
        };
        _hierarchyClient.BaseAddress = new Uri(_config.Value.HierarchyOdsApiUrl + "/");
        _odsClient.BaseAddress = new Uri(_config.Value.BaseOdsApiUrl + "/");
    }

    public async Task<List<Hierarchy>> GetHierarchiesFromSpine(List<string> odsCodes)
    {
        var hierarchies = new List<Hierarchy>();
        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 5 };

        await Parallel.ForEachAsync(odsCodes, parallelOptions, async (odsCode, ct) =>
        {
            var hierarchy = await GetHierarchyFromSpine(odsCode);
            hierarchies.Add(hierarchy);
        });
        return hierarchies;
    }

    public async Task<Hierarchy> GetHierarchyFromSpine(string odsCode)
    {
        string? regionCode;
        string? parentCode;

        var icbCode = await GetOrganisationRelationshipCodeFromSpine(odsCode, SpineRelationship.RE6) ?? await GetOrganisationRelationshipCodeFromSpine(odsCode, SpineRelationship.RE5) ?? await GetOrganisationRelationshipCodeFromSpine(odsCode, SpineRelationship.RE4);
        parentCode = await GetOrganisationRelationshipCodeFromSpine(icbCode, SpineRelationship.RE5);
        if (parentCode != null)
        {
            regionCode = await GetOrganisationRelationshipCodeFromSpine(parentCode, SpineRelationship.RE2);
        }
        else
        {
            regionCode = await GetOrganisationRelationshipCodeFromSpine(icbCode, SpineRelationship.RE2);
        }

        var organisation = await _organisationService.GetOrganisation(odsCode);
        var icb = await _organisationService.GetOrganisation(icbCode);
        var higherHealthAuthority = await _organisationService.GetOrganisation(parentCode);
        var nationalGrouping = await _organisationService.GetOrganisation(regionCode);

        return new Hierarchy()
        {
            OdsCode = organisation != null ? organisation.OdsCode : odsCode,
            SiteName = organisation != null ? organisation.OrganisationName : ActiveInactiveConstants.NOTAVAILABLE,
            Postcode = organisation != null ? organisation.PostalAddress.PostCode : ActiveInactiveConstants.NOTAVAILABLE,
            IcbCode = icbCode,
            HigherHealthAuthorityCode = parentCode,
            NationalGroupingCode = regionCode,
            IcbName = icb != null ? icb.OrganisationName : icbCode,
            HigherHealthAuthorityName = higherHealthAuthority != null ? higherHealthAuthority.OrganisationName : parentCode,
            NationalGroupingName = nationalGrouping != null ? nationalGrouping.OrganisationName : regionCode
        };
    }

    private async Task<string> GetOrganisationRelationshipCodeFromSpine(string odsCode, SpineRelationship relationshipId)
    {
        var response = await _odsClient.GetAsync($"organisations/{odsCode}");
        if (response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync();
            var organisationRelationships = JsonConvert.DeserializeObject<OrganisationRelationship>(body, _options);
            var organisationRelationship = organisationRelationships.Organisation?.Rels?.Rel?.FirstOrDefault(x => x.id == relationshipId.ToString() && x.Status == "Active");
            if (organisationRelationship != null)
            {
                return organisationRelationship.Target.OrgId.extension;
            }
            return null;
        }
        return null;
    }

    public async Task<List<Hierarchy>> GetOrganisationHierarchy(List<string> odsCodes)
    {
        var hierarchies = new List<Hierarchy>();
        var parallelOptions = new ParallelOptions() { MaxDegreeOfParallelism = 5 };

        await Parallel.ForEachAsync(odsCodes, parallelOptions, async (odsCode, ct) =>
        {
            var hierarchy = await GetOrganisationHierarchy(odsCode);
            hierarchies.Add(hierarchy);
        });
        return hierarchies;
    }

    public async Task<Hierarchy> GetOrganisationHierarchy(string odsCode)
    {
        try
        {
            _bearerToken = await GetBearerToken();
            _hierarchyClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
            var organisation = await _organisationService.GetOrganisation(odsCode);
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
                    hierarchy.IcbName = (await _organisationService.GetOrganisation(hierarchy.IcbCode))?.OrganisationName;
                }

                baseCodeSystem = await GetResponse($"{_config.Value.HierarchyOdsBaseUrl}&code={postCode}&property={ICB}&property={NHSER}");

                if (baseCodeSystem != null)
                {
                    var partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == ICB) > 0)).ElementAtOrDefault(0);
                    hierarchy.HigherHealthAuthorityCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueString;
                    hierarchy.HigherHealthAuthorityName = (await _organisationService.GetOrganisation(hierarchy.HigherHealthAuthorityCode))?.OrganisationName;

                    partValue = (baseCodeSystem.Parameter.Where(x => x.Part != null && x.Part.Count(y => y.ValueCode == NHSER) > 0)).ElementAtOrDefault(0);
                    hierarchy.NationalGroupingCode = partValue == null ? null : partValue.Part?.FirstOrDefault(x => x.Name == "value").ValueString;
                    hierarchy.NationalGroupingName = (await _organisationService.GetOrganisation(hierarchy.NationalGroupingCode))?.OrganisationName;
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
