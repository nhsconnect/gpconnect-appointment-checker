using GpConnect.AppointmentChecker.Models.Search;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;

public interface ISearchService
{
    Task<List<SearchResultList>> ExecuteSearch(Models.Request.SearchRequest searchRequest);
}
