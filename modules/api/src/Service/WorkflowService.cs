using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response.Reporting;
using JsonFlatten;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
                    var mailboxReporting = new MailboxReporting()
                    {
                        OdsCode = odsCodesInScope[i],
                        SupplierName = routeReportRequest.ReportSource[i].SupplierName,
                        Hierarchy = routeReportRequest.ReportSource[i].OrganisationHierarchy
                    };

                    var workflowData = await GetWorkflowData(routeReportRequest.Workflow[0], odsCodesInScope[i]);
                    if (workflowData != null)
                    {
                        mailboxReporting.Status = workflowData.Status;
                    }
                    else
                    {
                        mailboxReporting.Status = ActiveInactiveConstants.NOTAVAILABLE;
                    }
                    var jsonStringSD = JsonConvert.SerializeObject(mailboxReporting);
                    var jObjectSD = JObject.Parse(jsonStringSD);
                    var jDictSD = jObjectSD.Flatten();

                    workflows.Add(jDictSD);
                }
                jsonData = JsonConvert.SerializeObject(workflows);
            }
            return jsonData;
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
