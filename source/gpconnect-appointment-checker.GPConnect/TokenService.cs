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
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

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

        public async Task<RequestParameters> ConstructRequestParameters(Uri requestUri, Spine providerSpineMessage, Organisation providerOrganisationDetails, Spine consumerSpineMessage, Organisation consumerOrganisationDetails, int spineMessageTypeId)
        {
            try
            {
                var spineConfiguration = await _configurationService.GetSpineConfiguration();
                var generalConfiguration = await _configurationService.GetGeneralConfiguration();
                var spineMessageType = (await _configurationService.GetSpineMessageTypes()).FirstOrDefault(x => x.SpineMessageTypeId == spineMessageTypeId);

                var userGuid = Guid.NewGuid().ToString();
                var userFamilyName = "...";
                var userGivenName = "...";

                var tokenHandler = new JwtSecurityTokenHandler
                {
                    SetDefaultTimesOnTokenCreation = false
                };

                var tokenIssuer = spineConfiguration.sds_hostname;
                var tokenAudience = providerSpineMessage.ssp_hostname;
                var tokenIssuedAt = DateTimeOffset.Now;
                var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

                var tokenDescriptor = BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
                AddRequestingDeviceClaim(requestUri, tokenDescriptor, generalConfiguration);
                AddRequestingOrganisationClaim(providerOrganisationDetails, tokenDescriptor);
                AddRequestingPractitionerClaim(requestUri, tokenDescriptor, userGuid, userFamilyName, userGivenName);

                var token = AddTokenHeader(tokenHandler, tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var requestParameters = new RequestParameters
                {
                    BearerToken = tokenString,
                    SspFrom = spineConfiguration.asid,
                    SspTo = providerSpineMessage.asid,
                    UseSSP = spineConfiguration.use_ssp,
                    SspHostname = spineConfiguration.ssp_hostname,
                    ConsumerODSCode = consumerOrganisationDetails.ODSCode,
                    ProviderODSCode = providerOrganisationDetails.ODSCode,
                    InteractionId = spineMessageType?.InteractionId,
                    SpineMessageTypeId = spineMessageTypeId
                };

                return requestParameters;
            }
            catch (Exception exc)
            {
                _logger.LogError("An error has occurred in trying to build the JWT security token", exc);
                throw;
            }
        }

        private static void AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor,
            string userGuid, string userFamilyName, string userGivenName)
        {
            tokenDescriptor.Claims.Add("requesting_practitioner", new RequestingPractitioner
            {
                resourceType = "Practitioner",
                id = userGuid,
                name = new List<Name>
                {
                    new Name
                    {
                        family = userFamilyName,
                        given = new List<string> {userGivenName}
                    }
                },
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/sds-user-id",
                        value = "UNK"
                    },
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/sds-role-profile-id",
                        value = "UNK"
                    },
                    new Identifier
                    {
                        system = $"{requestUri.AbsoluteUri}/user-id",
                        value = userGuid
                    }
                }
            });
        }

        private static void AddRequestingOrganisationClaim(Organisation organisationDetails,
            SecurityTokenDescriptor tokenDescriptor)
        {
            tokenDescriptor.Claims.Add("requesting_organization", new RequestingOrganisation
            {
                resourceType = "Organization",
                name = organisationDetails.OrganisationName,
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/ods-organization-code",
                        value = organisationDetails.ODSCode
                    }
                }
            });
        }

        private static void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor,
            General generalConfiguration)
        {
            tokenDescriptor.Claims.Add("requesting_device", new RequestingDevice
            {
                resourceType = "Device",
                model = generalConfiguration.ProductName,
                version = generalConfiguration.ProductVersion,
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = requestUri.AbsoluteUri,
                        value = requestUri.Host
                    }
                }
            });
        }

        private static SecurityTokenDescriptor BuildSecurityTokenDescriptor(string tokenIssuer, string tokenAudience,
            string userGuid, DateTimeOffset tokenIssuedAt, DateTimeOffset tokenExpiration)
        {
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = tokenIssuer,
                Audience = tokenAudience,
                Claims = new Dictionary<string, object>()
                {
                    {Constants.TokenRequestValues.ReasonForRequestKey, Constants.TokenRequestValues.ReasonForRequestValue},
                    {Constants.TokenRequestValues.RequestedScopeKey, Constants.TokenRequestValues.RequestedScopeValue},
                    {Constants.TokenRequestValues.TokenSubject, userGuid}
                },
                IssuedAt = tokenIssuedAt.DateTime,
                Expires = tokenExpiration.DateTime
            };
            return tokenDescriptor;
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
