using Microsoft.IdentityModel.Tokens;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ITokenDependencies
{
    Task AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor, string userGuid, string Sid, string hostIdentifier, bool isID);
    void AddRequestingOrganisationClaim(SecurityTokenDescriptor tokenDescriptor, string systemIdentifier);
    void AddRequestingRecordClaim(SecurityTokenDescriptor tokenDescriptor, string systemIdentifier);
    void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor);
    SecurityTokenDescriptor BuildSecurityTokenDescriptor(string tokenIssuer, string tokenAudience, string userGuid, DateTimeOffset tokenIssuedAt, DateTimeOffset tokenExpiration);
}
