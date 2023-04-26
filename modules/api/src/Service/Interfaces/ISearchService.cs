using GpConnect.AppointmentChecker.Api.DTO.Response;

namespace GpConnect.AppointmentChecker.Api.Service.Interfaces;

public interface ISearchService
{
    Task<IEnumerable<SearchResponse>> ExecuteSearch(DTO.Request.SearchRequest searchRequest);
}
