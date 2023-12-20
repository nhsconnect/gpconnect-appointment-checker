using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface IApplicationService
{
    public Task SynchroniseOrganisation(DTO.Response.Spine.Organisation request);
    public Task<SearchGroup> AddSearchGroup(DTO.Request.Application.SearchGroup request);
    public Task UpdateSearchGroup(int searchGroupId);
    public Task UpdateSearchResult(int searchResultId, SearchResponse searchResponse, double timeTaken);
    public Task<AddSearchResult> AddSearchResult(DTO.Request.Application.SearchResult request);
    public Task<SearchResult> GetSearchResult(int searchResultId);
    public Task<List<SearchResponse>> GetSearchResultByGroup(int searchGroupId);
    public Task<SearchGroup> GetSearchGroup(int searchGroupId);
}
