using System.Collections.Generic;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface ILogService
    {
        void AddErrorLog(DTO.Request.Logging.ErrorLog errorLog);
        DTO.Response.Logging.SpineMessage AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage);
        void UpdateSpineMessageLog(int spineMessageId, int searchResultId);
        void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest);
        List<DTO.Response.Logging.PurgeErrorLog> PurgeLogEntries();
    }
}
