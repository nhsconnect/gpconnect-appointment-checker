using GpConnect.AppointmentChecker.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IApplicationService
{
    Task<Organisation> GetOrganisationAsync(string odsCode);
    Task<SearchGroup> AddSearchGroup(Models.Request.SearchGroup request);
    Task UpdateSearchGroup(int searchGroupId, int userId);
    Task<SearchResult> AddSearchResult(Models.Request.SearchResult request);
    Task<SearchExport> AddSearchExport(Models.Request.SearchExport request);
    Task<SearchExport> GetSearchExport(int searchExportId, int userId);
    Task<SearchGroupExport> GetSearchGroupExport(int searchGroupId, int userId);
    Task<SearchGroup> GetSearchGroup(int searchGroupId, int userId);
    Task<SearchResult> GetSearchResult(int searchResultId, int userId);
    Task<List<SlotEntrySummary>> GetSearchResultByGroup(int searchGroupId, int userId);
}