using GpConnect.AppointmentChecker.Models.Request;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IExportService
{
    Task<FileStreamResult> ExportSearchResultFromDatabase(SearchExport searchExport);
    Task<FileStreamResult> ExportSearchGroupFromDatabase(SearchExport searchExport);
}
