using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;

        public TokenService(ILogger<TokenService> logger, IConfigurationService configurationService, ILogService logService)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
        }

        public async Task<string> GenerateToken(Uri requestUri, Spine spineMessage, Organisation organisationDetails)
        {
            var spineConfiguration = await _configurationService.GetSpineConfiguration();
            var generalConfiguration = await _configurationService.GetGeneralConfiguration();

            var requestingDevice = new RequestingDevice
            {
                ResourceType = "Device",
                Identifier = new List<Identifier>
                {
                    new Identifier {
                        System = requestUri.AbsoluteUri,
                        Value = requestUri.Host
                    }
                },
                Model = generalConfiguration.ProductName,
                Version = generalConfiguration.ProductVersion
            };

            var requestingOrganisation = new RequestingOrganisation
            {
                ResourceType = "Organization",
                Identifier = new List<Identifier>
                {
                    new Identifier {
                        System = spineMessage.SSPHostname,
                        Value = organisationDetails.ODSCode
                    }
                },
                Name = organisationDetails.OrganisationName
            };

            var requestingPractitioner = new RequestingPractitioner
            {
                ResourceType = "Practitioner",
                Id = organisationDetails.OrganisationId.ToString(),
                Identifier = new List<Identifier>
                {
                    new Identifier {
                        System = spineMessage.SSPHostname,
                        Value = organisationDetails.ODSCode
                    }
                },
                Name = new List<Name>
                {
                    new Name
                    {
                        Family = "Test",
                        Given = new List<string>
                        {
                            "Test"
                        },
                        Prefix = new List<string>
                        {
                            "Test"
                        }
                    }
                }
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            tokenHandler.TokenLifetimeInMinutes = 5;

            var tokenIssuer = spineConfiguration.SDSHostname;
            var tokenAudience = spineMessage.SSPHostname;
            var tokenIssuedAt = DateTimeOffset.UtcNow;
            var tokenExpiration = DateTimeOffset.UtcNow.AddMinutes(5);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenIssuer,
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, spineMessage.OrganisationId.ToString())
                }),
                Audience = tokenAudience,
                Claims = new Dictionary<string, object>()
                {
                    { Constants.TokenRequestValues.TokenExpiration, tokenExpiration.ToUnixTimeSeconds() },
                    { Constants.TokenRequestValues.IssuedAt, tokenIssuedAt.ToUnixTimeSeconds() },
                    { Constants.TokenRequestValues.ReasonForRequestKey, Constants.TokenRequestValues.ReasonForRequestValue },
                    { Constants.TokenRequestValues.RequestedScopeKey, Constants.TokenRequestValues.RequestedScopeValue },
                    { Constants.TokenRequestValues.RequestingDevice, JsonConvert.SerializeObject(requestingDevice) },
                    { Constants.TokenRequestValues.RequestingOrganization, JsonConvert.SerializeObject(requestingOrganisation) },
                    { Constants.TokenRequestValues.RequestingPractitioner, JsonConvert.SerializeObject(requestingPractitioner) }
            },
                IssuedAt = tokenIssuedAt.DateTime,
                Expires = tokenExpiration.DateTime
            };

            var token = AddTokenHeader(tokenHandler, tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private static JwtSecurityToken AddTokenHeader(JwtSecurityTokenHandler tokenHandler, SecurityTokenDescriptor tokenDescriptor)
        {
            var token = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);
            token.Header.Add(Constants.TokenRequestValues.TokenHeaderAlgorithmKey, Constants.TokenRequestValues.TokenHeaderAlgorithmValue);
            token.Header.Add(Constants.TokenRequestValues.TokenHeaderTypeKey, Constants.TokenRequestValues.TokenHeaderTypeValue);
            return token;
        }
    }
}
