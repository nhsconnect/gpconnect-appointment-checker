using GpConnect.AppointmentChecker.Api.DTO.Request;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IExportService
{
    Task<Stream> ExportSearchResultFromDatabase(ExportRequest request);
    Task<Stream> ExportSearchGroupFromDatabase(ExportRequest request);
}
