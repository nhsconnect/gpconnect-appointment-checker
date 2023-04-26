using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IApplicationService
{
    public Task<DTO.Response.Spine.Organisation> GetOrganisation(string odsCode);
    public Task SynchroniseOrganisation(DTO.Response.Spine.Organisation request);
    public Task<SearchGroup> AddSearchGroup(DTO.Request.Application.SearchGroup request);
    public Task UpdateSearchGroup(int searchGroupId, int userId);
    public Task<AddSearchResult> AddSearchResult(DTO.Request.Application.SearchResult request);
    public Task<AddSearchExport> AddSearchExport(DTO.Request.Application.SearchExport request);
    public Task<SearchExport> GetSearchExport(int searchExportId, int userId);
    public Task<List<SearchGroupExport>> GetSearchGroupExport(int searchGroupId, int userId);
    public Task<SearchGroup> GetSearchGroup(int searchGroupId, int userId);
    public Task<SearchResult> GetSearchResult(int searchResultId, int userId);
    public Task<List<SlotEntrySummary>> GetSearchResultByGroup(int searchGroupId, int userId);
}
