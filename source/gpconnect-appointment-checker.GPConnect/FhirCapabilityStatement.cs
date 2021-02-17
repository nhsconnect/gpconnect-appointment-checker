using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using gpconnect_appointment_checker.Helpers;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        private List<CapabilityStatementList> GetCapabilityStatement(List<RequestParametersList> requestParameterList, CancellationToken cancellationToken)
        {
            try
            {
                var processedCapabilityStatements = new ConcurrentBag<CapabilityStatementList>();
                Parallel.ForEach(requestParameterList, requestParameter =>
                {
                    var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                        x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectReadMetaData);
                    requestParameter.RequestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectReadMetaData;
                    requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

                    var stopWatch = new Stopwatch();
                    stopWatch.Start();

                    _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

                    var client = _httpClientFactory.CreateClient("GpConnectClient");
                    AddRequiredRequestHeaders(requestParameter.RequestParameters, client);
                    _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();

                    using var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = new Uri($"{AddSecureSpineProxy(requestParameter)}/metadata")
                    };

                    using var response = client.Send(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                    var contents = response.Content.ReadAsStringAsync(cancellationToken).Result;

                    _spineMessage.ResponsePayload = contents;
                    _spineMessage.ResponseStatus = response.StatusCode.ToString();
                    _spineMessage.RequestPayload = request.ToString();
                    _spineMessage.ResponseHeaders = response.Headers.ToString();
                    stopWatch.Stop();
                    _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                    _logService.AddSpineMessageLog(_spineMessage);

                    if (response.IsSuccessStatusCode)
                    {
                        processedCapabilityStatements.Add(new CapabilityStatementList
                        {
                            OdsCode = requestParameter.OdsCode,
                            CapabilityStatement = JsonConvert.DeserializeObject<CapabilityStatement>(contents)
                        });
                    }
                });
                return processedCapabilityStatements.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
            }
        }
    }
}
