using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.Configuration;
using gpconnect_appointment_checker.GPConnect.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class TokenService : ITokenService
    {
        private RequestParametersList PopulateResultsProvider(SpineList providerSpineMessage, Uri requestUri, List<OrganisationList> providerOrganisationDetails, string userGuid, JwtSecurityTokenHandler tokenHandler, List<OrganisationList> consumerOrganisationDetails, List<SpineList> consumerSpineMessages, SpineMessageType spineMessageType, int spineMessageTypeId, string consumerOrganisationType = "")
        {
            var tokenIssuer = _spineOptionsDelegate.CurrentValue.SpineFqdn;
            var tokenAudience = providerSpineMessage.Spine?.EndpointAddress;
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
                SspFrom = _spineOptionsDelegate.CurrentValue.AsId,
                SspTo = providerSpineMessage.Spine.AsId,
                UseSSP = _spineOptionsDelegate.CurrentValue.UseSSP,
                SspHostname = _spineOptionsDelegate.CurrentValue.SspHostname,
                ProviderODSCode = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerSpineMessage.OdsCode)?.Organisation?.OdsCode,
                ConsumerODSCode = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerSpineMessages.FirstOrDefault().OdsCode)?.Organisation?.OdsCode,
                InteractionId = spineMessageType?.InteractionId,
                SpineMessageTypeId = spineMessageTypeId,
                GPConnectConsumerOrganisationType = consumerOrganisationType,
                EndpointAddress = providerSpineMessage.Spine.EndpointAddress
            };

            return new RequestParametersList
            {
                RequestParameters = requestParameters,
                OdsCode = providerSpineMessage.OdsCode
            };
        }

        private RequestParametersList PopulateResultsConsumer(Uri requestUri, List<SpineList> providerSpineMessages, List<OrganisationList> providerOrganisationDetails, int spineMessageTypeId, SpineList consumerSpineMessage, SpineMessageType spineMessageType, string userGuid, JwtSecurityTokenHandler tokenHandler, string consumerOrganisationType = "")
        {
            var tokenIssuer = _spineOptionsDelegate.CurrentValue.SpineFqdn;
            var tokenAudience = providerSpineMessages.FirstOrDefault()?.Spine?.EndpointAddress;
            var tokenIssuedAt = DateTimeOffset.Now;
            var tokenExpiration = DateTimeOffset.Now.AddMinutes(5);

            var tokenDescriptor = BuildSecurityTokenDescriptor(tokenIssuer, tokenAudience, userGuid, tokenIssuedAt, tokenExpiration);
            AddRequestingDeviceClaim(requestUri, tokenDescriptor);
            AddRequestingOrganisationClaim(providerOrganisationDetails.FirstOrDefault()?.Organisation, tokenDescriptor);
            AddRequestingPractitionerClaim(requestUri, tokenDescriptor, userGuid);

            var token = AddTokenHeader(tokenHandler, tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            var requestParameters = new RequestParameters
            {
                BearerToken = tokenString,
                SspFrom = _spineOptionsDelegate.CurrentValue.AsId,
                SspTo = providerSpineMessages.FirstOrDefault()?.Spine?.AsId,
                UseSSP = _spineOptionsDelegate.CurrentValue.UseSSP,
                SspHostname = _spineOptionsDelegate.CurrentValue.SspHostname,
                ProviderODSCode = providerSpineMessages.FirstOrDefault()?.OdsCode,
                ConsumerODSCode = consumerSpineMessage?.OdsCode,
                InteractionId = spineMessageType?.InteractionId,
                SpineMessageTypeId = spineMessageTypeId,
                GPConnectConsumerOrganisationType = consumerOrganisationType,
                EndpointAddress = providerSpineMessages.FirstOrDefault()?.Spine?.EndpointAddress
            };

            return new RequestParametersList
            {
                RequestParameters = requestParameters,
                OdsCode = consumerSpineMessage.OdsCode
            };
        }

        public async Task<List<RequestParametersList>> ConstructRequestParameters(Uri requestUri, List<SpineList> providerSpineMessages, List<OrganisationList> providerOrganisationDetails, List<SpineList> consumerSpineMessages, List<OrganisationList> consumerOrganisationDetails, int spineMessageTypeId, string consumerOrganisationType = "")
        {
            try
            {
                var spineMessageType = _configurationService.GetSpineMessageTypes().FirstOrDefault(x => x.SpineMessageTypeId == spineMessageTypeId);

                var userGuid = Guid.NewGuid().ToString();
                var tokenHandler = new JwtSecurityTokenHandler
                {
                    SetDefaultTimesOnTokenCreation = false
                };

                var tasks = new ConcurrentBag<Task<RequestParametersList>>();

                if (providerSpineMessages.Count > consumerSpineMessages.Count)
                {
                    Parallel.ForEach(providerSpineMessages.Where(x => !x.ProviderEnabledForGpConnectAppointmentManagement), providerSpineMessage =>
                    {
                        tasks.Add(Task.FromResult(new RequestParametersList
                        {
                            OdsCode = providerSpineMessage.OdsCode
                        }));
                    });

                    Parallel.ForEach(providerSpineMessages.Where(x => x.ProviderEnabledForGpConnectAppointmentManagement), providerSpineMessage =>
                    {
                        tasks.Add(Task.FromResult(PopulateResultsProvider(providerSpineMessage, requestUri, providerOrganisationDetails, userGuid, tokenHandler, consumerOrganisationDetails, consumerSpineMessages, spineMessageType, spineMessageTypeId, consumerOrganisationType)));
                    });
                }
                else if (consumerSpineMessages.Count > providerSpineMessages.Count)
                {
                    Parallel.ForEach(consumerSpineMessages, consumerSpineMessage =>
                    {
                        tasks.Add(Task.FromResult(PopulateResultsConsumer(requestUri, providerSpineMessages, providerOrganisationDetails, spineMessageTypeId, consumerSpineMessage, spineMessageType, userGuid, tokenHandler, consumerOrganisationType)));
                    });
                }
                var results = await Task.WhenAll(tasks);
                return results.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
                throw;
            }
        }
    }
}
