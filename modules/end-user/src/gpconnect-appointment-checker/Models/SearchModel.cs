using GpConnect.AppointmentChecker.Models.Search;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.CustomValidations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using SlotEntrySummary = GpConnect.AppointmentChecker.Models.SlotEntrySummary;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel : SearchBaseModel
    {
        public IEnumerable<SelectListItem> DateRanges => GetDateRanges();
        public IEnumerable<SelectListItem> OrganisationTypes => GetOrganisationTypes().Result;

        public List<SlotEntrySummary> SearchResultsSummary { get; set; }

        [Required(ErrorMessage = SearchConstants.ProviderOdsCodeRequiredErrorMessage)]
        [RegularExpression(ValidationConstants.AlphaNumericCharactersWithLeadingTrailingSpacesAndCommaSpaceOnly, ErrorMessage = SearchConstants.ProviderOdsCodeValidErrorMessage)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("MaxNumberProviderCodesSearch", SearchConstants.ProviderOdsCodeMaxLengthErrorMessage, SearchConstants.ProviderOdsCodeMaxLengthMultiSearchNotEnabledErrorMessage)]
        [RepeatedCodesCheck(SearchConstants.ProviderOdsCodeRepeatedCodeErrorMessage)]
        public string ProviderOdsCode { get; set; }

        [RegularExpression(ValidationConstants.AlphaNumericCharactersWithLeadingTrailingSpacesAndCommaSpaceOnly, ErrorMessage = SearchConstants.ConsumerOdsCodeValidErrorMessage)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("MaxNumberConsumerCodesSearch", SearchConstants.ConsumerOdsCodeMaxLengthErrorMessage, SearchConstants.ConsumerOdsCodeMaxLengthMultiSearchNotEnabledErrorMessage)]
        [RepeatedCodesCheck(SearchConstants.ConsumerOdsCodeRepeatedCodErrorMessage)]
        public string? ConsumerOdsCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedOrganisationType { get; set; }

        public bool DisplayMultiSearchHelpText => MultiSearchEnabled;
        public bool ConsumerOrgTypeSearchEnabled => OrgTypeSearchEnabled;

        public int SearchInputBoxLength => MultiSearchEnabled ? 100 : 10;
        public string ProviderOdsCodeInputBoxLabel => MultiSearchEnabled ? 
            SearchConstants.SearchInputProviderOdsCodeMultiLabel : 
            SearchConstants.SearchInputProviderOdsCodeLabel;

        public string ProviderOdsCodeInputBoxHintText => MultiSearchEnabled ?
            SearchConstants.SearchInputProviderOdsCodeHintText : string.Empty;

        public string ConsumerOdsCodeInputBoxHintText => GetConsumerOdsCodeInputHelpText();

        private string GetConsumerOdsCodeInputHelpText()
        {
            if(MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputConsumerOdsCodeHintText;
            }
            if (MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputConsumerOdsCodeHintText;
            }
            return string.Empty;
        }

        public string ConsumerOdsCodeInputBoxLabel => GetConsumerOdsCodeInputBoxLabelText();

        private string GetConsumerOdsCodeInputBoxLabelText()
        {
            if(MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputConsumerMultiLabel;
            }
            if (MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputConsumerOdsCodeMultiLabel;
            }
            if (!MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputMustEnterConsumerOrgTypeHintText;
            }
            if (!MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.SearchInputConsumerOdsCodeLabel;
            }
            return string.Empty;
        }

        public List<string> ProviderOdsCodeAsList => ProviderOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        
        public List<string> ConsumerOdsCodeAsList => ConsumerOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        public bool HasMultipleProviderOdsCodes => ProviderOdsCodeAsList?.Count > 1;
        public bool HasMultipleConsumerOdsCodes => ConsumerOdsCodeAsList?.Count > 1;

        public string CleansedProviderOdsCodeInput => string.Join(" ", ProviderOdsCodeAsList).ToUpper();
        public string CleansedConsumerOdsCodeInput => ConsumerOdsCodeAsList?.Count > 0 ? string.Join(" ", ConsumerOdsCodeAsList).ToUpper() : string.Empty;

        public bool ValidSearchCombination => ((!HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes) 
                                               || (HasMultipleConsumerOdsCodes && !HasMultipleProviderOdsCodes) 
                                               || (HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes));

        public string MultipleSearchResultsLink { get; set; }        

        [BindProperty]
        public string SelectedDateRange { get; set; }

        public bool IsMultiSearch { get; set; } = false;

        public bool ProviderODSCodeFound { get; set; } = true;
        public bool ConsumerODSCodeFound { get; set; } = true;

        public bool ProviderASIDPresent { get; set; } = true;
        public bool ProviderEnabledForGpConnectAppointmentManagement { get; set; } = true;
        public bool ConsumerEnabledForGpConnectAppointmentManagement { get; set; } = true;

        public bool SlotSearchOk { get; set; } = true;
        public bool CapabilityStatementOk { get; set; } = true;

        public string SearchSummaryDetail { get; set; }

        public ProviderError ProviderError { get; set; }

        public bool LdapErrorRaised { get; set; }
    }
}