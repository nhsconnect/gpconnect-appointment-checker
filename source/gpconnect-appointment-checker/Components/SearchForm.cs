using gpconnect_appointment_checker.ViewModels.Search;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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

        public IViewComponentResult Invoke(List<SelectListItem> dateRanges, string providerODSCode, string consumerODSCode, string dateRange, string searchButtonText, string clearButtonText)
        {

            var form = new SearchForm
            {
                DateRanges = dateRanges,
                ProviderODSCode = providerODSCode,
                ConsumerODSCode = consumerODSCode,
                SelectedDateRange = dateRange,
                SearchButtonText = searchButtonText,
                ClearButtonText = clearButtonText
            };

            return View(form);
        }
    }
}
