using System.Collections.Generic;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface ILogService
    {
        void AddErrorLog(DTO.Request.Logging.ErrorLog errorLog);
        void AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage);
        void AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest);
        Task<List<DTO.Response.Logging.PurgeErrorLog>> PurgeLogEntries();
    }
}
