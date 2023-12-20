namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.Ldap;

public interface ILdapService
{
    public Task<DTO.Response.Spine.Organisation?> GetOrganisation(string odsCode);
    public Task<DTO.Response.Configuration.Spine> GetGpProviderEndpointAndPartyKeyByOdsCode(string odsCode);
    public Task<DTO.Response.Configuration.Spine> GetGpProviderAsIdByOdsCodeAndPartyKey(string odsCode, string partyKey);
    public Task<DTO.Response.Configuration.Spine> GetGpConsumerAsIdByOdsCode(string odsCode);
}