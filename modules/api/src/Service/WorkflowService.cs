using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using gpconnect_appointment_checker.api.DTO.Response.Reporting;
using JsonFlatten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq.Dynamic.Core;

namespace GpConnect.AppointmentChecker.Api.Service;

public class WorkflowService : IWorkflowService
{
    private readonly ILogger<WorkflowService> _logger;
    private readonly IHttpClientFactory _httpClientFactory;

    public WorkflowService(ILogger<WorkflowService> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException();
    }

    public async Task<string?> CreateWorkflowData<T>(RouteReportRequest routeReportRequest) where T : class
    {
        try
        {
            var odsCodesInScope = routeReportRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var workflows = new List<IDictionary<string, object>>();

            if (odsCodesInScope.Count > 0)
            {
                for (var i = 0; i < odsCodesInScope.Count; i++)
                {
                    switch (typeof(T))
                    {
                        case Type type when type == typeof(UpdateRecordReporting):
                            var updateRecordReporting = new UpdateRecordReporting()
                            {
                                SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                                Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy
                            };

                            var workflowData = await GetWorkflowData(routeReportRequest.Workflow[0], odsCodesInScope[i]);
                            if (workflowData != null)
                            {
                                updateRecordReporting.Status = workflowData.Status;
                            }
                            else
                            {
                                updateRecordReporting.Status = ActiveInactiveConstants.NOTAVAILABLE;
                            }
                            var jsonStringUR = JsonConvert.SerializeObject(updateRecordReporting);
                            var jObjectUR = JObject.Parse(jsonStringUR);
                            workflows.Add(jObjectUR.Flatten());
                            break;
                    }
                }
                jsonData = JsonConvert.SerializeObject(workflows);
            }
            return jsonData != null ? jsonData.Substring(1, jsonData.Length - 2) : null;
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateWorkflowData'");
            throw;
        }
    }

    public async Task<DTO.Response.Mesh.Root?> GetWorkflowData(string workflow, string odsCode)
    {
        var getRequest = new HttpRequestMessage(HttpMethod.Get, $"{odsCode}/{workflow}");
        try
        {
            var client = _httpClientFactory.CreateClient(Clients.MESHCLIENT);
            var response = await client.SendAsync(getRequest);
            var responseStream = await response.Content.ReadAsStringAsync();

            var meshResponse = default(DTO.Response.Mesh.Root);

            if (responseStream.IsJson())
            {
                meshResponse = JsonConvert.DeserializeObject<DTO.Response.Mesh.Root>(responseStream);
            }
            return meshResponse;

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An error occurred in trying to execute a GetWorkflowData request - {getRequest}");
            throw;
        }
    }
}
