using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.CustomValidations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel
    {
        public IEnumerable<SelectListItem> DateRanges => GetDateRanges();
        public IEnumerable<SelectListItem> OrganisationTypes => GetOrganisationTypes();

        public List<List<SlotEntrySimple>> SearchResults { get; set; }
        public List<SlotEntrySummary> SearchResultsSummary { get; set; }

        public int MaxNumberOfCodesForMultiSearch => GetMaxNumberOfCodesForMultiSearch();

        [Required(ErrorMessage = SearchConstants.PROVIDERODSCODEREQUIREDERRORMESSAGE)]
        [RegularExpression(ValidationConstants.ALPHANUMERICCHARACTERSWITHLEADINGTRAILINGSPACESANDCOMMASPACEONLY, ErrorMessage = SearchConstants.PROVIDERODSCODEVALIDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("max_number_provider_codes_search", SearchConstants.PROVIDERODSCODEMAXLENGTHERRORMESSAGE, SearchConstants.PROVIDERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE, 20)]
        [RepeatedCodesCheck(SearchConstants.PROVIDERODSCODEREPEATEDCODERRORMESSAGE)]
        public string ProviderOdsCode { get; set; }

        [RegularExpression(ValidationConstants.ALPHANUMERICCHARACTERSWITHLEADINGTRAILINGSPACESANDCOMMASPACEONLY, ErrorMessage = SearchConstants.CONSUMERODSCODEVALIDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("max_number_provider_codes_search", SearchConstants.CONSUMERODSCODEMAXLENGTHERRORMESSAGE, SearchConstants.CONSUMERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE, 20)]
        [RepeatedCodesCheck(SearchConstants.CONSUMERODSCODEREPEATEDCODERRORMESSAGE)]
        public string ConsumerOdsCode { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SelectedOrganisationType { get; set; }

        public bool DisplayMultiSearchHelpText => _multiSearchEnabled;
        public bool ConsumerOrgTypeSearchEnabled => _orgTypeSearchEnabled;

        public int SearchInputBoxLength => _multiSearchEnabled ? 100 : 10;
        public string ProviderOdsCodeInputBoxLabel => _multiSearchEnabled ? 
            SearchConstants.SEARCHINPUTPROVIDERODSCODEMULTILABEL : 
            SearchConstants.SEARCHINPUTPROVIDERODSCODELABEL;

        public string ProviderOdsCodeInputBoxHintText => _multiSearchEnabled ?
            SearchConstants.SEARCHINPUTPROVIDERODSCODEHINTTEXT : string.Empty;

        public string ConsumerOdsCodeInputBoxHintText => GetConsumerOdsCodeInputHelpText();

        private string GetConsumerOdsCodeInputHelpText()
        {
            if(_multiSearchEnabled && _orgTypeSearchEnabled)
            {
                return SearchConstants.SEARCHINPUTCONSUMERRODSCODEHINTTEXT;
            }
            if (_multiSearchEnabled && !_orgTypeSearchEnabled)
            {
                return SearchConstants.SEARCHINPUTCONSUMERRODSCODEHINTTEXT;
            }
            if (!_multiSearchEnabled && _orgTypeSearchEnabled)
            {
                return SearchConstants.SEARCHINPUTMUSTENTERCONSUMERORGTYPEHINTTEXT;
            }
            return string.Empty;
        }

        public string ConsumerOdsCodeInputBoxLabel => GetConsumerOdsCodeInputBoxLabelText();

        private string GetConsumerOdsCodeInputBoxLabelText()
        {
            if(_multiSearchEnabled && _orgTypeSearchEnabled)
            {
                return SearchConstants.SEARCHINPUTCONSUMERMULTILABEL;
            }
            if (_multiSearchEnabled && !_orgTypeSearchEnabled)
            {
                return SearchConstants.SEARCHINPUTCONSUMERODSCODEMULTILABEL;
            }
            return SearchConstants.SEARCHINPUTCONSUMERODSCODELABEL;
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

        public bool IsMultiSearch => HasMultipleProviderOdsCodes || HasMultipleConsumerOdsCodes;

        [BindProperty]
        public string SearchAtResultsText { get; set; }
        [BindProperty]
        public string SearchOnBehalfOfResultsText { get; set; }
        [BindProperty]
        public string SelectedDateRange { get; set; }       

        [BindProperty(Name = "SearchGroupId", SupportsGet = true)]
        public int SearchGroupId { get; set; }

        public double SearchDuration { get; set; }
        public bool ProviderODSCodeFound { get; set; } = true;
        public bool ConsumerODSCodeFound { get; set; } = true;

        public bool ProviderASIDPresent { get; set; } = true;
        public bool ProviderEnabledForGpConnectAppointmentManagement { get; set; } = true;
        public bool ConsumerEnabledForGpConnectAppointmentManagement { get; set; } = true;

        public bool SlotSearchOk { get; set; } = true;
        public bool CapabilityStatementOk { get; set; } = true;
        public string ProviderErrorDisplay { get; set; }
        public string ProviderErrorCode { get; set; }
        public string ProviderErrorDiagnostics { get; set; }
        public int? SearchResultsCount { get; set; }
        public bool LdapErrorRaised { get; set; }
        public string ProviderPublisher { get; set; }
    }
}