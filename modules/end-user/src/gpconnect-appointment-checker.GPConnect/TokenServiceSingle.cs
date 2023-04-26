//using gpconnect_appointment_checker.DAL.Interfaces;
//using gpconnect_appointment_checker.DTO.Request.GpConnect;
//using gpconnect_appointment_checker.DTO.Response.Application;
//using gpconnect_appointment_checker.DTO.Response.Configuration;
//using gpconnect_appointment_checker.GPConnect.Interfaces;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using System;
//using System.IdentityModel.Tokens.Jwt;
//using System.Linq;

//namespace gpconnect_appointment_checker.GPConnect
//{
//    public partial class TokenService : ITokenService
//    {
//        private readonly ILogger<TokenService> _logger;
//        private readonly ILogService _logService;
//        private readonly IConfigurationService _configurationService;
//        private readonly IHttpContextAccessor _context;
//        private readonly IOptionsMonitor<General> _generalOptionsDelegate;
//        private readonly IOptionsMonitor<Spine> _spineOptionsDelegate;

//        public TokenService(ILogger<TokenService> logger, IConfigurationService configurationService, ILogService logService, IHttpContextAccessor context, IOptionsMonitor<General> generalOptionsDelegate, IOptionsMonitor<Spine> spineOptionsDelegate)
//        {
//            _logger = logger;
//            _configurationService = configurationService;
//            _logService = logService;
//            _context = context;
//            _generalOptionsDelegate = generalOptionsDelegate;
//            _spineOptionsDelegate = spineOptionsDelegate;
//        }

//        public RequestParameters ConstructRequestParameters(Uri requestUri, Spine providerSpineDetails, Organisation providerOrganisationDetails, Spine consumerEnablement, Organisation consumerOrganisationDetails, int spineMessageTypeId, string consumerOrganisationType = "")
//        {
//            try
//            {
//                var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x => x.SpineMessageTypeId == spineMessageTypeId);

//                var userGuid = Guid.NewGuid().ToString();
//                var tokenHandler = new JwtSecurityTokenHandler
//                {
//                    SetDefaultTimesOnTokenCreation = false
//                };

//                var tokenIssuer = _spineOptionsDelegate.CurrentValue.SpineFqdn;
//                var tokenAudience = providerSpineDetails.EndpointAddress;
//                var tokenIssuedAt = DateTimeOffset.Now;
//                var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

//                var tokenDescriptor = BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
//                AddRequestingDeviceClaim(requestUri, tokenDescriptor);
//                AddRequestingOrganisationClaim(providerOrganisationDetails, tokenDescriptor);
//                AddRequestingPractitionerClaim(requestUri, tokenDescriptor, userGuid);

//                var token = AddTokenHeader(tokenHandler, tokenDescriptor);
//                var tokenString = tokenHandler.WriteToken(token);

//                var requestParameters = new RequestParameters
//                {
//                    BearerToken = tokenString,
//                    SspFrom = _spineOptionsDelegate.CurrentValue.AsId,
//                    SspTo = providerSpineDetails.AsId,
//                    UseSSP = _spineOptionsDelegate.CurrentValue.UseSSP,
//                    EndpointAddress = providerSpineDetails.EndpointAddress,
//                    ConsumerODSCode = consumerOrganisationDetails?.OdsCode,
//                    ProviderODSCode = providerOrganisationDetails.OdsCode,
//                    InteractionId = spineMessageType?.InteractionId,
//                    SpineMessageTypeId = spineMessageTypeId,
//                    GPConnectConsumerOrganisationType = consumerOrganisationType,
//                    SspHostname = _spineOptionsDelegate.CurrentValue.SspHostname
//                };
//                return requestParameters;
//            }
//            catch (Exception exc)
//            {
//                _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
//                throw;
//            }
//        }
//    }
//}
