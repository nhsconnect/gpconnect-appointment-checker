using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers.Constants;
using gpconnect_appointment_checker.Helpers.CustomValidations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Pages
{
    public partial class SearchModel
    {
        public List<SelectListItem> DateRanges => GetDateRanges();

        public List<List<SlotEntrySimple>> SearchResults { get; set; }
        public List<SlotEntrySummary> SearchResultsSummary { get; set; }

        public int MaxNumberOfCodesForMultiSearch => GetMaxNumberOfCodesForMultiSearch();

        [Required(ErrorMessage = SearchConstants.PROVIDERODSCODEREQUIREDERRORMESSAGE)]
        [RegularExpression(ValidationConstants.ALPHANUMERICCHARACTERSWITHLEADINGTRAILINGSPACESANDCOMMASPACEONLY, ErrorMessage = SearchConstants.PROVIDERODSCODEVALIDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("max_number_provider_codes_search", SearchConstants.PROVIDERODSCODEMAXLENGTHERRORMESSAGE, SearchConstants.PROVIDERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE, 20)]
        //[RepeatedCodesCheck(SearchConstants.PROVIDERODSCODEREPEATEDCODERRORMESSAGE)]
        public string ProviderOdsCode { get; set; }

        [Required(ErrorMessage = SearchConstants.CONSUMERODSCODEREQUIREDERRORMESSAGE)]
        [RegularExpression(ValidationConstants.ALPHANUMERICCHARACTERSWITHLEADINGTRAILINGSPACESANDCOMMASPACEONLY, ErrorMessage = SearchConstants.CONSUMERODSCODEVALIDERRORMESSAGE)]
        [BindProperty(SupportsGet = true)]
        [MaximumNumberOfCodes("max_number_provider_codes_search", SearchConstants.CONSUMERODSCODEMAXLENGTHERRORMESSAGE, SearchConstants.CONSUMERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE, 20)]
        //[RepeatedCodesCheck(SearchConstants.CONSUMERODSCODEREPEATEDCODERRORMESSAGE)]
        public string ConsumerOdsCode { get; set; }

        public int SearchInputBoxLength => _multiSearchEnabled ? 100 : 10;
        public string ProviderOdsCodeInputBoxLabel => _multiSearchEnabled ? 
            SearchConstants.SEARCHINPUTPROVIDERODSCODEMULTILABEL : 
            SearchConstants.SEARCHINPUTPROVIDERODSCODELABEL;

        public string ProviderOdsCodeInputBoxHintText => _multiSearchEnabled ?
            SearchConstants.SEARCHINPUTPROVIDERODSCODEHINTTEXT : string.Empty;

        public string ConsumerOdsCodeInputBoxHintText => _multiSearchEnabled ?
            SearchConstants.SEARCHINPUTCONSUMERRODSCODEHINTTEXT : string.Empty;

        public string ConsumerOdsCodeInputBoxLabel => _multiSearchEnabled ? 
            SearchConstants.SEARCHINPUTCONSUMERODSCODEMULTILABEL : 
            SearchConstants.SEARCHINPUTCONSUMERODSCODELABEL;

        public List<string> ProviderOdsCodeAsList => ProviderOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        
        public List<string> ConsumerOdsCodeAsList => ConsumerOdsCode?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToList();

        public bool HasMultipleProviderOdsCodes => ProviderOdsCodeAsList?.Count > 1;
        public bool HasMultipleConsumerOdsCodes => ConsumerOdsCodeAsList?.Count > 1;

        public string CleansedProviderOdsCodeInput => string.Join(" ", ProviderOdsCodeAsList).ToUpper();
        public string CleansedConsumerOdsCodeInput => string.Join(" ", ConsumerOdsCodeAsList).ToUpper();

        public bool ValidSearchCombination => ((!HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes) 
                                               || (HasMultipleConsumerOdsCodes && !HasMultipleProviderOdsCodes) 
                                               || (HasMultipleProviderOdsCodes && !HasMultipleConsumerOdsCodes));

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