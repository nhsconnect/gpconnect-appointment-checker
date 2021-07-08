using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using gpconnect_appointment_checker.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ILogService _logService;
        private readonly IConfigurationService _configurationService;
        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _context;

        public TokenService(ILogger<TokenService> logger, IConfigurationService configurationService, ILogService logService, IConfiguration configuration, IHttpContextAccessor context)
        {
            _logger = logger;
            _configurationService = configurationService;
            _logService = logService;
            _configuration = configuration;
            _context = context;
        }

        public List<RequestParametersList> ConstructRequestParameters(Uri requestUri, List<SpineList> providerSpineMessages, List<OrganisationList> providerOrganisationDetails, List<SpineList> consumerSpineMessages, List<OrganisationList> consumerOrganisationDetails, int spineMessageTypeId)
        {
            try
            {
                var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x => x.SpineMessageTypeId == spineMessageTypeId);

                var userGuid = Guid.NewGuid().ToString();
                var tokenHandler = new JwtSecurityTokenHandler
                {
                    SetDefaultTimesOnTokenCreation = false
                };

                var requestParameterList = new ConcurrentBag<RequestParametersList>();

                foreach (var providerSpineMessage in providerSpineMessages.Where(x => !x.ProviderEnabledForGpConnectAppointmentManagement))
                {
                    requestParameterList.Add(new RequestParametersList
                    {
                        OdsCode = providerSpineMessage.OdsCode
                    });
                }

                Parallel.ForEach(providerSpineMessages.Where(x => x.ProviderEnabledForGpConnectAppointmentManagement), providerSpineMessage =>
                {
                    var tokenIssuer = _configuration.GetSection("Spine:spine_fqdn").Value;
                    var tokenAudience = providerSpineMessage.Spine.ssp_hostname;
                    var tokenIssuedAt = DateTimeOffset.Now;
                    var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

                    var tokenDescriptor = BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
                    AddRequestingDeviceClaim(requestUri, tokenDescriptor);
                    AddRequestingOrganisationClaim(providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerSpineMessage.OdsCode)?.Organisation, tokenDescriptor);
                    AddRequestingPractitionerClaim(requestUri, tokenDescriptor, userGuid);

                    var token = AddTokenHeader(tokenHandler, tokenDescriptor);
                    var tokenString = tokenHandler.WriteToken(token);

                    var requestParameters = new RequestParameters
                    {
                        BearerToken = tokenString,
                        SspFrom = _configuration.GetSection("Spine:uniqueIdentifier").Value,
                        SspTo = providerSpineMessage.Spine.asid,
                        UseSSP = bool.Parse(_configuration.GetSection("Spine:use_ssp").Value),
                        SspHostname = _configuration.GetSection("Spine:nhsMHSEndPoint").Value,
                        ConsumerODSCode = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerSpineMessage.OdsCode)?.Organisation.ODSCode,
                        ProviderODSCode = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerSpineMessage.OdsCode)?.Organisation.ODSCode,
                        InteractionId = spineMessageType?.InteractionId,
                        SpineMessageTypeId = spineMessageTypeId
                    };
                    requestParameterList.Add(new RequestParametersList
                    {
                        RequestParameters = requestParameters,
                        BaseAddress = providerSpineMessage.Spine.ssp_hostname,
                        OdsCode = providerSpineMessage.OdsCode
                    });
                });
                return requestParameterList.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
                throw;
            }
        }

        public RequestParameters ConstructRequestParameters(Uri requestUri, Spine providerSpineMessage, Organisation providerOrganisationDetails, Spine consumerEnablement, Organisation consumerOrganisationDetails, int spineMessageTypeId)
        {
            try
            {
                var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x => x.SpineMessageTypeId == spineMessageTypeId);

                var userGuid = Guid.NewGuid().ToString();
                var tokenHandler = new JwtSecurityTokenHandler
                {
                    SetDefaultTimesOnTokenCreation = false
                };

                var tokenIssuer = _configuration.GetSection("Spine:spine_fqdn").Value;
                var tokenAudience = providerSpineMessage.ssp_hostname;
                var tokenIssuedAt = DateTimeOffset.Now;
                var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

                var tokenDescriptor = BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
                AddRequestingDeviceClaim(requestUri, tokenDescriptor);
                AddRequestingOrganisationClaim(providerOrganisationDetails, tokenDescriptor);
                AddRequestingPractitionerClaim(requestUri, tokenDescriptor, userGuid);

                var token = AddTokenHeader(tokenHandler, tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                var requestParameters = new RequestParameters
                {
                    BearerToken = tokenString,
                    SspFrom = _configuration.GetSection("Spine:uniqueIdentifier").Value,
                    SspTo = providerSpineMessage.asid,
                    UseSSP = bool.Parse(_configuration.GetSection("Spine:use_ssp").Value),
                    SspHostname = _configuration.GetSection("Spine:nhsMHSEndPoint").Value,
                    ConsumerODSCode = consumerOrganisationDetails.ODSCode,
                    ProviderODSCode = providerOrganisationDetails.ODSCode,
                    InteractionId = spineMessageType?.InteractionId,
                    SpineMessageTypeId = spineMessageTypeId
                };
                return requestParameters;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
                throw;
            }
        }

        private void AddRequestingPractitionerClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor,
            string userGuid)
        {
            var familyName = _context.HttpContext.User.GetClaimValue("FamilyName");
            var givenName = _context.HttpContext.User.GetClaimValue("GivenName");
            var displayName = _context.HttpContext.User.GetClaimValue("DisplayName");
            var nameParts = Regex.Split(displayName, @"[^a-zA-Z0-9]").Where(x => x != string.Empty).ToArray();

            tokenDescriptor.Claims.Add("requesting_practitioner", new RequestingPractitioner
            {
                resourceType = "Practitioner",
                id = userGuid,
                name = new List<Name>
                {
                    new Name
                    {
                        family = StringExtensions.Coalesce(familyName, nameParts[0].FirstCharToUpper(true)),
                        given = new List<string> { StringExtensions.Coalesce(givenName, nameParts[1].FirstCharToUpper(true)) }
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
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/email-address",
                        value = _context.HttpContext.User.GetClaimValue("Email")
                    },
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/nhsmail-sid",
                        value = _context.HttpContext.User.GetClaimValue("sid")
                    }
                }
            });
        }

        private void AddRequestingOrganisationClaim(Organisation organisationDetails,
            SecurityTokenDescriptor tokenDescriptor)
        {
            var odsCode = _configuration.GetSection("Spine:ods_code").Value;
            var organisationName = _configuration.GetSection("Spine:organisation_name").Value;

            tokenDescriptor.Claims.Add("requesting_organization", new RequestingOrganisation
            {
                resourceType = "Organization",
                name = organisationName,
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://fhir.nhs.uk/Id/ods-organization-code",
                        value = odsCode
                    }
                }
            });
        }

        private void AddRequestingDeviceClaim(Uri requestUri, SecurityTokenDescriptor tokenDescriptor)
        {
            tokenDescriptor.Claims.Add("requesting_device", new RequestingDevice
            {
                resourceType = "Device",
                model = _configuration.GetSection("General:product_name").Value,
                version = _configuration.GetSection("General:product_version").Value,
                identifier = new List<Identifier>
                {
                    new Identifier
                    {
                        system = "https://appointmentchecker.gpconnect.nhs.uk/Id/device-identifier",
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
            return token;
        }
    }
}
