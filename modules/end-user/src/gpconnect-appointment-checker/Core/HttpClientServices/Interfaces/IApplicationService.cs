using GpConnect.AppointmentChecker.Models;
using GpConnect.AppointmentChecker.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface IApplicationService
{
    Task<SearchGroup> GetSearchGroup(int searchGroupId);
    Task<SearchResult> GetSearchResult(int searchResultId);
    Task<List<SearchResultList>> GetSearchResultByGroup(int searchGroupId);
}