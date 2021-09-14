using gpconnect_appointment_checker.DTO;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers.Enumerations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface IGpConnectQueryExecutionService
    {
        Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress, int userId);
        SlotSimple ExecuteFreeSlotSearchFromDatabase(string responseStream, int userId);
        Task<List<SlotEntrySummaryCount>> ExecuteFreeSlotSearchSummary(List<OrganisationErrorCodeOrDetail> organisationErrorCodeOrDetail, List<RequestParametersList> requestParameters, DateTime startDate, DateTime endDate, SearchType searchType);
        Task<List<CapabilityStatementList>> ExecuteFhirCapabilityStatement(List<RequestParametersList> requestParameters);
        Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress);
        void SendToAudit(List<string> auditSearchParameters, List<string> auditSearchIssues, Stopwatch stopWatch, bool isMultiSearch, int? resultCount = 0);
    }
}
