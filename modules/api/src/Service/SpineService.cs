using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.DTO.Response.Spine;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Fhir;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;
using Microsoft.Extensions.Options;

namespace GpConnect.AppointmentChecker.Api.Service;

public class SpineService : ISpineService
{
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;
    private readonly IFhirService _fhirService;
    private readonly ILdapService _ldapService;

    public SpineService(IFhirService fhirService, ILdapService ldapService, IOptions<SpineConfig> spineOptionsDelegate)
    {
        _fhirService = fhirService ?? throw new ArgumentNullException();
        _ldapService = ldapService ?? throw new ArgumentNullException();
        _spineOptionsDelegate = spineOptionsDelegate ?? throw new ArgumentNullException();
    }

    public async Task<Organisation?> GetOrganisationDetailsByOdsCodeAsync(string odsCode)
    {
        if (_spineOptionsDelegate.Value.SdsUseFhirApi)
        {
            return await _fhirService.GetOrganisation(odsCode);
        }
        return await _ldapService.GetOrganisation(odsCode);
    }

    public async Task<Spine> GetProviderDetails(string odsCode, string? interactionId = null)
    {
        if (_spineOptionsDelegate.Value.SdsUseFhirApi)
        {
            var spineProviderDetails = await _fhirService.GetProviderDetails(odsCode, interactionId ?? _spineOptionsDelegate.Value.DefaultInteraction);
            return LoadAdditionalDependencies(spineProviderDetails);
        }
        else
        {
            var spineProviderDetails = await _ldapService.GetGpProviderEndpointAndPartyKeyByOdsCode(odsCode);
            if (spineProviderDetails != null)
            {
                var spineProviderAsId = await _ldapService.GetGpProviderAsIdByOdsCodeAndPartyKey(odsCode, spineProviderDetails.PartyKey);
                spineProviderDetails.AsId = spineProviderAsId.AsId;
                spineProviderDetails.ProductName = spineProviderAsId.ProductName;
            }
            return LoadAdditionalDependencies(spineProviderDetails);
        }
    }

    public async Task<Spine> GetConsumerDetails(string odsCode)
    {
        if (_spineOptionsDelegate.Value.SdsUseFhirApi)
        {
            return await _fhirService.GetConsumerDetails(odsCode);
        }
        else
        {
            return await _ldapService.GetGpConsumerAsIdByOdsCode(odsCode);
        }
    }

    private Spine LoadAdditionalDependencies(Spine spine)
    {
        if (spine != null)
        {
            spine.SpineFqdn = _spineOptionsDelegate.Value.SpineFqdn;
            spine.SspFrom = _spineOptionsDelegate.Value.AsId;
            spine.SspHostname = _spineOptionsDelegate.Value.SspHostname;
            spine.UseSSP = _spineOptionsDelegate.Value.UseSSP;
        }
        return spine;
    }

}
