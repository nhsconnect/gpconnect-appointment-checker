using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.GPConnect.Constants;
using gpconnect_appointment_checker.Helpers.Enumerations;
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

namespace gpconnect_appointment_checker.GPConnect
{
    public partial class GpConnectQueryExecutionService
    {
        private async Task<List<CapabilityStatementList>> GetCapabilityStatement(List<RequestParametersList> requestParameterList, CancellationToken cancellationToken)
        {
            try
            {
                var emptyRequestParameters = requestParameterList.Where(x => x.RequestParameters == null)
                    .Select(requestParameter => new CapabilityStatementList
                    {
                        OdsCode = requestParameter.OdsCode,
                        ErrorCode = ErrorCode.CapabilityStatementNotFound
                    }).ToArray();

                var requestList = requestParameterList.Where(x => x.RequestParameters != null && x.RequestParameters.EndpointAddress != null);

                var client = _httpClientFactory.CreateClient("GpConnectClient");
                var semaphore = new SemaphoreSlim(requestList.Count(), requestList.Count() + 1);

                var tasks = requestList.Select(requestParameter => PopulateCapabilityStatementResults(requestParameter, semaphore, client, cancellationToken));

                var results = await Task.WhenAll(tasks);
                return emptyRequestParameters.Concat(results).ToList();
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, "An error occurred in trying to execute a GET request");
                throw;
            }
        }

        private async Task<CapabilityStatementList> PopulateCapabilityStatementResults(RequestParametersList requestParameter, SemaphoreSlim semaphore, HttpClient client, CancellationToken cancellationToken)
        {
            try
            {
                var spineMessageType = (_configurationService.GetSpineMessageTypes()).FirstOrDefault(x =>
                        x.SpineMessageTypeId == (int)SpineMessageTypes.GpConnectReadMetaData);
                requestParameter.RequestParameters.SpineMessageTypeId =
                    (int)SpineMessageTypes.GpConnectReadMetaData;
                requestParameter.RequestParameters.InteractionId = spineMessageType?.InteractionId;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                _spineMessage.SpineMessageTypeId = requestParameter.RequestParameters.SpineMessageTypeId;

                AddRequiredRequestHeaders(requestParameter.RequestParameters, client);
                _spineMessage.RequestHeaders = client.DefaultRequestHeaders.ToString();

                await semaphore.WaitAsync();

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{requestParameter.RequestParameters.EndpointAddressWithSpineSecureProxy}/metadata")
                };

                var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
                var contents = await response.Content.ReadAsStringAsync();              

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
            finally
            {
                semaphore.Release();
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
                getRequest.RequestUri = new Uri($"{requestParameters.EndpointAddressWithSpineSecureProxy}/metadata");

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
