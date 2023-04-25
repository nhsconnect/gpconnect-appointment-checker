using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Spine;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;
using Novell.Directory.Ldap;
using System.Text.RegularExpressions;

namespace GpConnect.AppointmentChecker.Api.Service.Ldap;

public class LdapService : ILdapService
{
    private readonly ILogger<LdapService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IApplicationService _applicationService;
    private readonly ILdapRequestExecution _ldapRequestExecution;

    public LdapService(ILogger<LdapService> logger, IApplicationService applicationService, IConfigurationService configurationService, ILdapRequestExecution ldapRequestExecution)
    {
        _logger = logger;
        _applicationService = applicationService ?? throw new ArgumentNullException(nameof(applicationService));
        _configurationService = configurationService ?? throw new ArgumentNullException(nameof(configurationService));
        _ldapRequestExecution = ldapRequestExecution ?? throw new ArgumentNullException(nameof(ldapRequestExecution));
    }

    public async Task<Spine> GetGpConsumerAsIdByOdsCode(string odsCode)
    {
        try
        {
            var sdsQuery = await GetSdsQueryByName(LdapQuery.GetGpConsumerAsIdByOdsCode);
            var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
            var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);
            var spine = response != null ? new Spine
            {
                EndpointAddress = response.EndpointAddress,
                AsId = response.AsId,
                PartyKey = response.PartyKey
            } : null;

            return spine;
        }
        catch (LdapException ldapException)
        {
            _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
            throw;
        }
    }

    public async Task<Spine> GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey)
    {
        try
        {
            var sdsQuery = await GetSdsQueryByName(LdapQuery.GetGpProviderAsIdByOdsCodeAndPartyKey);
            var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)).Replace("{partyKey}", Regex.Escape(partyKey));
            var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);

            var spine = response != null ? new Spine
            {
                AsId = response.AsId,
                PartyKey = response.PartyKey,
                ProductName = response.ProductName
            } : null;

            return spine;
        }
        catch (LdapException ldapException)
        {
            _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
            throw;
        }
    }

    public async Task<Spine> GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode)
    {
        try
        {
            var sdsQuery = await GetSdsQueryByName(LdapQuery.GetGpProviderEndpointAndPartyKeyByOdsCode);
            var filter = sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode));
            var response = _ldapRequestExecution.ExecuteLdapQuery<DTO.Response.Ldap.Spine>(sdsQuery.SearchBase, filter, sdsQuery.QueryAttributesAsArray);

            var spine = response != null ? new Spine
            {
                EndpointAddress = response.EndpointAddress,
                AsId = response.AsId,
                PartyKey = response.PartyKey
            } : null;

            return spine;
        }
        catch (LdapException ldapException)
        {
            _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error has occurred while attempting to execute an LDAP query");
            throw;
        }
    }

    public async Task<Organisation> GetOrganisation(string odsCode)
    {
        if (!string.IsNullOrEmpty(odsCode))
        {
            var sdsQuery = await GetSdsQueryByName(LdapQuery.GetOrganisationDetailsByOdsCode);
            var organisation = _ldapRequestExecution.ExecuteLdapQuery<Organisation>(sdsQuery.SearchBase, sdsQuery.QueryText.Replace("{odsCode}", Regex.Escape(odsCode)), sdsQuery.QueryAttributesAsArray);
            await _applicationService.SynchroniseOrganisation(organisation);
            return organisation;
        }
        return null;
    }

    private async Task<SdsQuery> GetSdsQueryByName(string queryName)
    {
        try
        {
            var sdsQuery = await _configurationService.GetSdsQueryConfiguration(queryName);
            return sdsQuery;
        }
        catch (LdapException ldapException)
        {
            _logger.LogError(ldapException, "An LdapException error has occurred while attempting to execute an LDAP query");
            throw;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error has occurred while attempting to load LDAP queries from the database");
            throw;
        }
    }
}
