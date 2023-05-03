using GpConnect.AppointmentChecker.Api.DTO.Response;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface ISearchService
{
    Task<List<SearchResponse>> ExecuteSearch(DTO.Request.SearchRequest searchRequest);
}
