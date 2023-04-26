using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;

public interface IGpConnectQueryExecutionService
{
    Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int userId);
    Task<SlotSimple> ExecuteFreeSlotSearchFromDatabase(string responseStream, int userId);
    //Task<List<SlotEntrySummaryCount>> ExecuteFreeSlotSearchSummary(List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetail, List<RequestParametersList> requestParameters, DateTime startDate, DateTime endDate, SearchType searchType);
    //Task<List<CapabilityStatementList>> ExecuteFhirCapabilityStatement(List<RequestParametersList> requestParameters);
    Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress);
    //void SendToAudit(List<string> auditSearchParameters, List<string> auditSearchIssues, Stopwatch stopWatch, bool isMultiSearch, int? resultCount = 0);
}
