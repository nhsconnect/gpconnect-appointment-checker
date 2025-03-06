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

        [Required(ErrorMessage = SearchConstants.Providerodscoderequirederrormessage)]
        [RegularExpression(ValidationConstants.AlphaNumericCharactersWithLeadingTrailingSpacesAndCommaSpaceOnly, ErrorMessage = SearchConstants.Providerodscodevaliderrormessage)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("MaxNumberProviderCodesSearch", SearchConstants.Providerodscodemaxlengtherrormessage, SearchConstants.Providerodscodemaxlengthmultisearchnotenablederrormessage)]
        [RepeatedCodesCheck(SearchConstants.Providerodscoderepeatedcoderrormessage)]
        public string ProviderOdsCode { get; set; }

        [RegularExpression(ValidationConstants.AlphaNumericCharactersWithLeadingTrailingSpacesAndCommaSpaceOnly, ErrorMessage = SearchConstants.Consumerodscodevaliderrormessage)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("MaxNumberConsumerCodesSearch", SearchConstants.Consumerodscodemaxlengtherrormessage, SearchConstants.Consumerodscodemaxlengthmultisearchnotenablederrormessage)]
        [RepeatedCodesCheck(SearchConstants.Consumerodscoderepeatedcoderrormessage)]
        public string? ConsumerOdsCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string? SelectedOrganisationType { get; set; }

        public bool DisplayMultiSearchHelpText => MultiSearchEnabled;
        public bool ConsumerOrgTypeSearchEnabled => OrgTypeSearchEnabled;

        public int SearchInputBoxLength => MultiSearchEnabled ? 100 : 10;
        public string ProviderOdsCodeInputBoxLabel => MultiSearchEnabled ? 
            SearchConstants.Searchinputproviderodscodemultilabel : 
            SearchConstants.Searchinputproviderodscodelabel;

        public string ProviderOdsCodeInputBoxHintText => MultiSearchEnabled ?
            SearchConstants.Searchinputproviderodscodehinttext : string.Empty;

        public string ConsumerOdsCodeInputBoxHintText => GetConsumerOdsCodeInputHelpText();

        private string GetConsumerOdsCodeInputHelpText()
        {
            if(MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputconsumerrodscodehinttext;
            }
            if (MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputconsumerrodscodehinttext;
            }
            return string.Empty;
        }

        public string ConsumerOdsCodeInputBoxLabel => GetConsumerOdsCodeInputBoxLabelText();

        private string GetConsumerOdsCodeInputBoxLabelText()
        {
            if(MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputconsumermultilabel;
            }
            if (MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputconsumerodscodemultilabel;
            }
            if (!MultiSearchEnabled && OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputmustenterconsumerorgtypehinttext;
            }
            if (!MultiSearchEnabled && !OrgTypeSearchEnabled)
            {
                return SearchConstants.Searchinputconsumerodscodelabel;
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