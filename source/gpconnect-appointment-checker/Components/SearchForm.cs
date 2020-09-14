using gpconnect_appointment_checker.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace gpconnect_appointment_checker.Components
{
    [ViewComponent(Name = "SearchForm")]
    public class SearchFormVC : ViewComponent
    {
        private readonly IConfiguration _configuration;
        private IHttpContextAccessor _contextAccessor;

        public SearchFormVC(IConfiguration configuration, IHttpContextAccessor contextAccessor)
        {
            _configuration = configuration;
            _contextAccessor = contextAccessor;
        }

        public IViewComponentResult Invoke(List<SelectListItem> dateRanges, SearchResultItemList searchResults, string providerODSCode, string consumerODSCode, string dateRange, string searchButtonText, string clearButtonText, string searchAtText, string searchOnBehalfOfText, string dateRangeText, string searchResultsHeadingText, string resultColumns)
        {

            var form = new SearchForm
            {
                DateRanges = dateRanges,
                ProviderODSCode = providerODSCode,
                ConsumerODSCode = consumerODSCode,
                SelectedDateRange = dateRange,
                SearchButtonText = searchButtonText,
                ClearButtonText = clearButtonText,
                SearchAtText = searchAtText,
                SearchOnBehalfOfText = searchOnBehalfOfText,
                DateRangeText = dateRangeText,
                SearchResultsHeadingText = searchResultsHeadingText,
                ResultColumns = resultColumns.Split(","),
                SearchResults = searchResults
            };

            return View(form);
        }
    }
}
