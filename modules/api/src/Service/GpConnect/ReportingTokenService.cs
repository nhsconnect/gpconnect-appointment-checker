﻿using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace GpConnect.AppointmentChecker.Api.Service.GpConnect;

public class ReportingTokenService : IReportingTokenService
{
    private readonly ILogger<TokenService> _logger;
    private readonly IConfigurationService _configurationService;
    private readonly IReportingTokenDependencies _reportingTokenDependencies;
    private readonly IOptions<SpineConfig> _spineOptionsDelegate;

    public ReportingTokenService(ILogger<TokenService> logger, IConfigurationService configurationService, IOptions<SpineConfig> spineOptionsDelegate, IReportingTokenDependencies reportingTokenDependencies)
    {
        _logger = logger;
        _configurationService = configurationService;
        _spineOptionsDelegate = spineOptionsDelegate;
        _reportingTokenDependencies = reportingTokenDependencies;
    }

    public async Task<DTO.Response.GpConnect.RequestParameters> ConstructRequestParameters(DTO.Request.GpConnect.RequestParameters request, string? interactionId = null, bool isID = true)
    {
        try
        {            
            var spineMessageType = await _configurationService.GetSpineMessageType(request.SpineMessageTypeId, interactionId);

            var userGuid = Guid.NewGuid().ToString();
            var tokenHandler = new JwtSecurityTokenHandler
            {
                SetDefaultTimesOnTokenCreation = false
            };

            var tokenIssuer = _spineOptionsDelegate.Value.SpineFqdn;
            var tokenAudience = request.AuthenticationAudience ?? request.ProviderSpineDetails.EndpointAddress;
            var tokenIssuedAt = DateTimeOffset.Now;
            var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

            var tokenDescriptor = _reportingTokenDependencies.BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
            _reportingTokenDependencies.AddRequestingDeviceClaim(request.RequestUri, tokenDescriptor);
            _reportingTokenDependencies.AddRequestingOrganisationClaim(tokenDescriptor, request.SystemIdentifier);
            _reportingTokenDependencies.AddRequestingRecordClaim(tokenDescriptor, request.SystemIdentifier);
            await _reportingTokenDependencies.AddRequestingPractitionerClaim(request.RequestUri, tokenDescriptor, userGuid, request.Sid, request.HostIdentifier, isID);

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
