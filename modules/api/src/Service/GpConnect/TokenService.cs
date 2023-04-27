using GpConnect.AppointmentChecker.Api.DTO.Response.Configuration;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class TokenService : ITokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly ITokenDependencies _tokenDependencies;
    private readonly IOptions<Spine> _spineOptionsDelegate;

    public TokenService(ILogger<TokenService> logger, IConfigurationService configurationService, IOptions<Spine> spineOptionsDelegate, ITokenDependencies tokenDependencies)
    {
        _logger = logger;
        _configurationService = configurationService;
        _spineOptionsDelegate = spineOptionsDelegate;
        _tokenDependencies = tokenDependencies;
    }

    public async Task<DTO.Response.GpConnect.RequestParameters> ConstructRequestParameters(DTO.Request.GpConnect.RequestParameters request)
    {
        try
        {
            
            var spineMessageType = await _configurationService.GetSpineMessageType(request.SpineMessageTypeId);

            var userGuid = Guid.NewGuid().ToString();
            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            var tokenIssuer = _spineOptionsDelegate.Value.SpineFqdn;
            var tokenAudience = request.ProviderSpineDetails.EndpointAddress;
            var tokenIssuedAt = DateTimeOffset.Now;
            var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

            var tokenDescriptor = _tokenDependencies.BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
            _tokenDependencies.AddRequestingDeviceClaim(request.RequestUri, tokenDescriptor);
            _tokenDependencies.AddRequestingOrganisationClaim(tokenDescriptor);
            await _tokenDependencies.AddRequestingPractitionerClaim(request.RequestUri, tokenDescriptor, userGuid, request.UserId, request.Sid);

            var token = AddTokenHeader(tokenHandler, tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var response = new DTO.Response.GpConnect.RequestParameters
            {
                BearerToken = tokenString,
                SspFrom = _spineOptionsDelegate.Value.AsId,
                SspTo = request.ProviderSpineDetails.AsId,
                UseSSP = _spineOptionsDelegate.Value.UseSSP,
                EndpointAddress = request.ProviderSpineDetails.EndpointAddress,
                ConsumerODSCode = request.ConsumerOrganisationDetails?.OdsCode,
                ProviderODSCode = request.ProviderOrganisationDetails.OdsCode,
                InteractionId = spineMessageType?.InteractionId,
                SpineMessageTypeId = request.SpineMessageTypeId,
                GPConnectConsumerOrganisationType = request.ConsumerOrganisationType,
                SspHostname = _spineOptionsDelegate.Value.SspHostname,
                RequestTimeout = _spineOptionsDelegate.Value.TimeoutSeconds
            };
            return response;
        }
        catch (Exception exc)
        {
            _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
            throw;
        }
    }

    private static JwtSecurityToken AddTokenHeader(JwtSecurityTokenHandler tokenHandler, SecurityTokenDescriptor tokenDescriptor)
    {
        var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
        return token;
    }
}
