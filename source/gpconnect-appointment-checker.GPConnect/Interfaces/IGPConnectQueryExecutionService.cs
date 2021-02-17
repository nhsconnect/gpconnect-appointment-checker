using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface IGpConnectQueryExecutionService
    {
        List<SlotSimple> ExecuteFreeSlotSearch(List<RequestParametersList> requestParameters, DateTime startDate, DateTime endDate);
        List<SlotSummary> ExecuteFreeSlotSearchSummary(List<RequestParametersList> requestParameters, DateTime startDate, DateTime endDate);
        List<CapabilityStatementList> ExecuteFhirCapabilityStatement(List<RequestParametersList> requestParameters);
        void SendToAudit(List<string> auditSearchParameters, List<string> auditSearchIssues, Stopwatch stopWatch, int? resultCount = 0);
    }
}
