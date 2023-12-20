using GpConnect.AppointmentChecker.Models.Request;
using GpConnect.AppointmentChecker.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface ISearchService
{
    Task<List<SearchResultList>> ExecuteSearch(SearchRequest searchRequest);
    Task<SearchResultList> ExecuteFreeSlotSearchFromDatabase(SearchRequestFromDatabase searchRequestFromDatabase);
}
