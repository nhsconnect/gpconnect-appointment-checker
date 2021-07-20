using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        private CapabilityStatementList PopulateCapabilityStatementResults(RequestParametersList requestParameter, CancellationToken cancellationToken)
        {
            var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                    x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectReadMetaData);
            requestParameter.RequestParameters.SpineMessageTypeId =
                (int)SpineMessageTypes.GpConnectReadMetaData;
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

            var capabilityStatement = JsonConvert.DeserializeObject<CapabilityStatement>(contents);

            return new CapabilityStatementList
            {
                OdsCode = requestParameter.OdsCode,
                CapabilityStatement = capabilityStatement,
                ErrorCode = (capabilityStatement.Issue?.Count > 0 ? ErrorCode.CapabilityStatementHasErrors : ErrorCode.None)
            };
        }

        private async Task<List<CapabilityStatementList>> GetCapabilityStatement(List<RequestParametersList> requestParameterList, CancellationToken cancellationToken)
        {
            try
            {
                var tasks = new ConcurrentBag<Task<CapabilityStatementList>>();

                Parallel.ForEach(requestParameterList.Where(x => x.RequestParameters == null), requestParameter =>
                {
                    tasks.Add(Task.FromResult(new CapabilityStatementList
                    {
                        OdsCode = requestParameter.OdsCode,
                        ErrorCode = ErrorCode.CapabilityStatementNotFound
                    }));
                });

                Parallel.ForEach(requestParameterList.Where(x => x.RequestParameters != null && x.BaseAddress != null), requestParameter =>
                {
                    tasks.Add(Task.FromResult(PopulateCapabilityStatementResults(requestParameter, cancellationToken)));
                });

                var results = await Task.WhenAll(tasks);
                return results.ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
            }
        }

        private async Task<CapabilityStatement> GetCapabilityStatement(RequestParameters requestParameters, string baseAddress)
        {
            var getRequest = new HttpRequestMessage();
            var stopWatch = new Stopwatch();

            try
            {
                var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                    x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectReadMetaData);
                requestParameters.SpineMessageTypeId = (int)SpineMessageTypes.GpConnectReadMetaData;
                requestParameters.InteractionId = spineMessageType?.InteractionId;

                _spineMessage.SpineMessageTypeId = requestParameters.SpineMessageTypeId;

                var client = _httpClientFactory.CreateClient("GpConnectClient");
                AddRequiredRequestHeaders(requestParameters, client);
                _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();

                getRequest.Method = HttpMethod.Get;
                getRequest.RequestUri = new Uri($"{AddSecureSpineProxy(baseAddress, requestParameters)}/metadata");

                _spineMessage.RequestPayload = getRequest.ToString();

                stopWatch.Start();
                var response = await client.SendAsync(getRequest);
                var responseStream = await response.Content.ReadAsStringAsync();

                _spineMessage.ResponsePayload = responseStream;
                _spineMessage.ResponseStatus = response.StatusCode.ToString();
                _spineMessage.ResponseHeaders = response.Headers.ToString();

                var results = JsonConvert.DeserializeObject<CapabilityStatement>(responseStream);
                return results;
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"An error occurred in trying to execute a GET request - {getRequest}");
                throw;
            }
            finally
            {
                stopWatch.Stop();
                _spineMessage.RoundTripTimeMs = stopWatch.ElapsedMilliseconds;
                _logService.AddSpineMessageLog(_spineMessage);
            }
        }
    }
}
