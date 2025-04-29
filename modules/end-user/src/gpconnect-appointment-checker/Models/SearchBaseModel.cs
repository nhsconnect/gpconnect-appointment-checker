using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Text;

namespace gpconnect_appointment_checker.Pages
{
    public class SearchBaseModel : BaseModel
    {
        public SearchBaseModel(IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {
        }

        public List<List<SearchResultEntry>> SearchResultsCurrent { get; set; }
        public List<List<SearchResultEntry>> SearchResultsPast { get; set; }

        public List<SearchResultList> SearchResultsAll { get; set; }

        [BindProperty]
        public string? SearchAtResultsText { get; set; }
        [BindProperty]
        public string? SearchOnBehalfOfResultsText { get; set; }
        
        [BindProperty(Name = "SearchGroupId", SupportsGet = true)]
        public int SearchGroupId { get; set; }

        [BindProperty(Name = "SearchResultId", SupportsGet = true)]
        public int SearchResultId { get; set; }

        public double SearchDuration { get; set; }

        public int SearchResultsTotalCount { get; set; } = 0;
        public int SearchResultsCurrentCount { get; set; } = 0;
        public int SearchResultsPastCount { get; set; } = 0;

        public string ProviderPublisher { get; set; }

        public string GetSearchOnBehalfOfResultsText(string consumerFormattedOrganisationDetails, string selectedOrganisationType)
        {
            var searchOnBehalfOfResultsText = new StringBuilder();

            if (!string.IsNullOrEmpty(consumerFormattedOrganisationDetails))
            {
                searchOnBehalfOfResultsText.Append($"<p>{consumerFormattedOrganisationDetails}</p>");
            }
            if (!string.IsNullOrEmpty(selectedOrganisationType))
            {
                searchOnBehalfOfResultsText.Append($"<p><em>{SearchConstants.SearchResultsSearchOnbehalfOfOrgTypeText}</em>&nbsp;{selectedOrganisationType}</p>");
            }
            return searchOnBehalfOfResultsText.ToString();
        }
    }
}
