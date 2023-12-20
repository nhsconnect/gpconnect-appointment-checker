namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface ILogService
{
    Task AddErrorLog(DTO.Request.Logging.ErrorLog errorLog);
    Task<DTO.Response.Logging.SpineMessage> AddSpineMessageLog(DTO.Request.Logging.SpineMessage spineMessage);
    Task<DTO.Response.Logging.SpineMessage> GetSpineMessageLogBySearchResultId(int searchResultId);
    Task<List<DTO.Response.Logging.SpineMessage>> GetSpineMessageLogBySearchGroupId(int searchGroupId);
    Task UpdateSpineMessageLog(int spineMessageId, int searchResultId);
    Task AddWebRequestLog(DTO.Request.Logging.WebRequest webRequest);
    Task<List<DTO.Response.Logging.PurgeErrorLog>> PurgeLogEntries();
}
