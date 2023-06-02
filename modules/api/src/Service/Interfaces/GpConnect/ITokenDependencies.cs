using Microsoft.IdentityModel.Tokens;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface ITokenDependencies
{
    Task AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor, string userGuid, string Sid);
    void AddRequestingOrganisationClaim(SecurityTokenDescriptor tokenDescriptor);
    void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor);
    SecurityTokenDescriptor BuildSecurityTokenDescriptor(string tokenIssuer, string tokenAudience, string userGuid, DateTimeOffset tokenIssuedAt, DateTimeOffset tokenExpiration);
}
