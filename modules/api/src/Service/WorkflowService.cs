using GpConnect.AppointmentChecker.Api.Core.Configuration;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using gpconnect_appointment_checker.api.DTO.Response.Reporting;
using JsonFlatten;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Linq.Dynamic.Core;

namespace GpConnect.AppointmentChecker.Api.Service;

public class WorkflowService : IWorkflowService
{
    private readonly ILogger<WorkflowService> _logger;
    private readonly IOrganisationService _organisationService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOptions<MeshConfig> _meshOptionsDelegate;

    public WorkflowService(ILogger<WorkflowService> logger, IOptions<MeshConfig> meshOptionsDelegate, IHttpClientFactory httpClientFactory, IOrganisationService organisationService, IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _organisationService = organisationService ?? throw new ArgumentNullException();
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException();
        _meshOptionsDelegate = meshOptionsDelegate ?? throw new ArgumentNullException();
    }

    public async Task<string> CreateWorkflowData(RouteReportRequest routeReportRequest)
    {
        try
        {
            _logger.LogInformation("Trying to generate WorkflowService.CreateWorkflowData: " + routeReportRequest.ReportName);

            var odsCodesInScope = routeReportRequest.ReportSource.DistinctBy(x => x.OdsCode).Select(x => x.OdsCode).ToList();
            string? jsonData = null;
            var organisationHierarchy = await _organisationService.GetOrganisationHierarchy(odsCodesInScope);
            var workflows = new List<IDictionary<string, object>>();

            if (odsCodesInScope.Count > 0)
            {
                for (var i = 0; i < odsCodesInScope.Count; i++)
                {
                    var workflowReporting = new WorkflowReporting()
                    {
                        SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                        Hierarchy = organisationHierarchy[odsCodesInScope[i]]
                    };

                    _logger.LogInformation("Executing GetWorkflowData: " + routeReportRequest.Workflow[0]);
                    _logger.LogInformation("Executing GetWorkflowData: " + odsCodesInScope[i]);

                    var workflowData = await GetWorkflowData(routeReportRequest.Workflow[0], odsCodesInScope[i]);
                    if (workflowData != null)
                    {
                        workflowReporting.Active = workflowData.Active;
                    }

                    var jsonString = JsonConvert.SerializeObject(workflowReporting);
                    var jObject = JObject.Parse(jsonString);
                    workflows.Add(jObject.Flatten());
                }
                jsonData = JsonConvert.SerializeObject(workflows);
            }
            return jsonData.Substring(1, jsonData.Length - 2);
        }
        catch (Exception exc)
        {
            _logger?.LogError(exc, "An error has occurred while attempting to execute the function 'CreateWorkflowData'");
            throw;
        }
    }

    private async Task<DTO.Response.Mesh.Root?> GetWorkflowData(string workflow, string odsCode)
    {
        var getRequest = new HttpRequestMessage();

        try
        {
            var client = _httpClientFactory.CreateClient(Clients.MESHCLIENT);            
            getRequest.Method = HttpMethod.Get;
            getRequest.RequestUri = new Uri($"{_meshOptionsDelegate.Value.MeshHostname}/{_meshOptionsDelegate.Value.EndpointAddress}/{odsCode}/{workflow}");

            _logger.LogInformation("Executing GetWorkflowData Request: " + getRequest.RequestUri.ToString());            

            var response = await client.SendAsync(getRequest);
            var responseStream = await response.Content.ReadAsStringAsync();

            _logger.LogInformation("Getting GetWorkflowData Response: " + responseStream);

            var meshResponse = default(DTO.Response.Mesh.Root);

            if (responseStream.IsJson())
            {
                meshResponse = JsonConvert.DeserializeObject<DTO.Response.Mesh.Root>(responseStream);
            }
            return meshResponse;

        }
        catch (Exception exc)
        {
            _logger.LogError(exc, $"An error occurred in trying to execute a GET request - {getRequest}");
            throw;
        }
    }
}
