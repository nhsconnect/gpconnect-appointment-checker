using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using gpconnect_appointment_checker.DTO.Request.GpConnect;
using gpconnect_appointment_checker.DTO.Response.GpConnect;

namespace gpconnect_appointment_checker.GPConnect.Interfaces
{
    public interface IGPConnectQueryExecutionService
    {
        Task<SlotSimple> ExecuteFreeSlotSearch(RequestParameters requestParameters, DateTime startDate, DateTime endDate, string baseAddress);
        Task<CapabilityStatement> ExecuteFhirCapabilityStatement(RequestParameters requestParameters, string baseAddress);
    }
}
