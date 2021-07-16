﻿using gpconnect_appointment_checker.DTO.Request.GpConnect;
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

                if (providerSpineMessages.Count > consumerSpineMessages.Count)
                {
                    Parallel.ForEach(providerSpineMessages.Where(x => !x.ProviderEnabledForGpConnectAppointmentManagement), providerSpineMessage =>
                    {
                        requestParameterList.Add(new RequestParametersList
                        {
                            OdsCode = providerSpineMessage.OdsCode
                        });
                    });

                    Parallel.ForEach(providerSpineMessages.Where(x => x.ProviderEnabledForGpConnectAppointmentManagement), providerSpineMessage =>
                    {
                        var tokenIssuer = _configuration.GetSection("Spine:spine_fqdn").Value;
                        var tokenAudience = providerSpineMessage.Spine?.ssp_hostname;
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
                            ProviderODSCode = providerOrganisationDetails.FirstOrDefault(x => x.OdsCode == providerSpineMessage.OdsCode)?.Organisation?.ODSCode,
                            ConsumerODSCode = consumerOrganisationDetails.FirstOrDefault(x => x.OdsCode == consumerSpineMessages.FirstOrDefault().OdsCode)?.Organisation?.ODSCode,
                            InteractionId = spineMessageType?.InteractionId,
                            SpineMessageTypeId = spineMessageTypeId
                        };
                        requestParameterList.Add(new RequestParametersList
                        {
                            RequestParameters = requestParameters,
                            BaseAddress = providerSpineMessage.Spine?.ssp_hostname,
                            OdsCode = providerSpineMessage.OdsCode
                        });
                    });
                }
                else if (consumerSpineMessages.Count > providerSpineMessages.Count)
                {
                    Parallel.ForEach(consumerSpineMessages, consumerSpineMessage =>
                    {
                        var tokenIssuer = _configuration.GetSection("Spine:spine_fqdn").Value;
                        var tokenAudience = providerSpineMessages.FirstOrDefault()?.Spine?.ssp_hostname;
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
                            SspFrom = _configuration.GetSection("Spine:uniqueIdentifier").Value,
                            SspTo = providerSpineMessages.FirstOrDefault()?.Spine?.asid,
                            UseSSP = bool.Parse(_configuration.GetSection("Spine:use_ssp").Value),
                            SspHostname = _configuration.GetSection("Spine:nhsMHSEndPoint").Value,
                            ProviderODSCode = providerSpineMessages.FirstOrDefault()?.OdsCode,
                            ConsumerODSCode = consumerSpineMessage.OdsCode,
                            InteractionId = spineMessageType?.InteractionId,
                            SpineMessageTypeId = spineMessageTypeId
                        };
                        requestParameterList.Add(new RequestParametersList
                        {
                            RequestParameters = requestParameters,
                            BaseAddress = providerSpineMessages.FirstOrDefault()?.Spine?.ssp_hostname,
                            OdsCode = consumerSpineMessage.OdsCode
                        });
                    });
                }
                return requestParameterList.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error has occurred in trying to build the JWT security token");
                throw;
            }
        }
    }
}
