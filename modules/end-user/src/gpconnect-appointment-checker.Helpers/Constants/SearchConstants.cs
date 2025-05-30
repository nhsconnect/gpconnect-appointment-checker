namespace gpconnect_appointment_checker.Helpers.Constants
{
    public class SearchConstants
    {
        public const string IssueWithDdsCodesLabel = "Issue with ODS codes";
        public const string IssueWithConsumerOdscodeLabel = "Issue with Consumer ODS code";
        public const string IssueWithProviderOdscodeLabel = "Issue with Provider ODS code";
        public const string IssueWithGpConnectProviderLabel = "Issue with GP Connect Provider";
        public const string IssueWithGpConnectConsumerLabel = "Issue with GP Connect Consumer";

        public const string IssueWithSendingMessageToProviderSystemLabel =
            "An error occured when sending a message to the provider system";

        public const string IssueWithLdapLabel = "An LDAP error has occurred";

        public const string MultipleCodeHintText =
            "Please be aware that the search may take a while if multiple codes have been entered.";

        public const string AccessLabel = "Access to GP Connect Appointment Checker";

        public const string AccessText =
            "You must be authorised to use this application. Please contact <a href=\"mailto:{0}?subject=Appointment%20checker%20user%20auth%20request\">{0}</a> to request access, providing your name, role and organisation.";

        public const string AccessGrantedText =
            "Once you have been granted access, please click <a href=\"{0}\">here</a> to return to the login page.";

        public const string AccessTextAlt =
            "This application is restricted to registered users of the NHS. Please sign in to use it.";

        public const string AccessTextRegisterAlt =
            "If you don't have an account, please <a href=\"{0}\">" + CreateAccountButtonText + "</a>.";

        public const string NotRegisteredText = "You are not registered to use the Appointment Checker.";

        public const string NotRegisteredLinkText =
            "Please <a href=\"{0}\">" + CreateAccountButtonText + "</a> for an account.";

        public const string CreateAccountLabel = "Register for GP Connect Appointment Checker";

        public const string CreateAccountText =
            "<p>Your NHS Mail email address is needed to register to use the GP Connect Appointment Checker.</p><p>Please sign in with your NHS Mail account to start the registration process.</p>";

        public const string CreateAccountTextHome =
            "Or <a href=\"{0}\">Cancel</a> to return to the home page.";

        public const string CreateAccountCancelText = "If you do not wish to continue, please <a href=\"{0}\">" +
                                                      CancelText + "</a> to return to the home page.";

        public const string Usernamelabel = "User name";
        public const string JobRoleLabel = "Job role";
        public const string OrganisationLabel = "Organisation (if different to the one shown above)";
        public const string AccessRequestReasonLabel = "Reason for requesting access to the Appointment Checker.";

        public const string AccessRequestReasonHintText =
            "Please provide as much information as possible, this will allow your account to be created quickly. For example, \"I am responsible for setting up slots across my PCN and want to check them.\"";

        public const string SignedInText = "Signed in to GP Connect Appointment Checker as {0}";
        public const string RunSearchText = "Run a search";

        public const string AlreadyHaveAnAccountText =
            "You already have an account and can use the Appointment Checker";

        public const string SearchText = "Search";
        public const string AdminText = "Admin";
        public const string ReportsText = "Reports";
        public const string HomeText = "Home";
        public const string HelpText = "Help";
        public const string SigninButtonText = "Sign in";
        public const string CreateAccountButtonText = "Register";
        public const string CancelText = "Cancel";
        public const string RegisterToUseText = "Register to use the Appointment Checker";

        public const string PendingAccountText =
            "You have previously asked for an account to be created, and the request is pending. You will receive an email when the request has been authorised.";

        public const string AccountDisabledLabel = "Account Disabled";
        public const string RequestSubmittedLabel = "Request Submitted";

        public const string RequestSubmittedText =
            "Your request to create an account has been submitted. You will receive an email as soon as your account has been set up.";

        public const string RequestSubmittedErrorText =
            "There was a problem creating your Appointment Checker account. Please email <a href=\"mailto:{0}\">{0}</a> and your account will be set up manually.";

        public const string SubmitUserFormLabel = "Access Denied";

        public const string SubmitUserFormText =
            "You do not have the necessary authorisation to use the Appointment Checker, or a previous request was denied. You need to complete a short form and confirm that you have agreed to the Terms and Conditions. Click on the button below to continue.";

        public const string SignoutButtonText = "Sign out";
        public const string DeauthoriseButtonText = "De-authorise";
        public const string AuthoriseButtonText = "Authorise";

        public const string ApplyFilterButtonText = "Apply filters";

        public const string PrivacyAndCookiesText = "Privacy and Cookies";
        public const string AccessibilityStatementText = "Accessibility Statement";
        public const string TermsAndConditionsText = "Terms and Conditions";

        public const string IssueWithOdscodesInputLabel = "Issue with ODS codes entered";

        public const string IssueWithOdsCodesInputText =
            "Sorry, but you cannot enter multiple Provider ODS codes and multiple Consumer ODS codes at the same time.";

        public const string IssueWithOdsCodesText =
            "Sorry, but the Provider ODS code '{0}' and the Consumer ODS code '{1}' have not been found";

        public const string IssueWithConsumerOdsCodeText = "Sorry, but the Consumer ODS code '{0}' has not been found";
        public const string IssueWithProviderOdsCodeText = "Sorry, but the Provider ODS code '{0}' has not been found";
        public const string IssueWithGpConnectProviderText = "Sorry, this organisation is not enabled for GP Connect";

        public const string IssueWithGpConnectProviderNotEnabledText =
            "Sorry, but the Provider ODS code {0} is not enabled for GP Connect Appointment Management";

        public const string IssueWithGpConnectConsumerNotEnabledText =
            "The Consumer ODS code {0} is not enabled for GP Connect";

        public const string IssueWithSendingMessageToProviderSystemText = "The error returned was \"{0} ({1})\".";

        public const string IssueWithLdapText =
            "The SDS server is currently unavailable. Please try your search again.";

        public const string ErrorHeaderTitleText = "Error in GP Connect Appointment Checker";
        public const string StatusCodeErrorTitleText = "Status Code Error in GP Connect Appointment Checker";

        public const string ErrorText =
            "Sorry, but it looks like an error has occurred in the application. The error has been logged and will be investigated.";

        public const string ReturnToHomePageText = "Please click here to return to the Home page.";
        public const string ReturnToSearchPageText = "Please click here to return to the Search page.";

        public const string IssueWithTimeoutTitleText = "Issue with timeout";

        public const string IssueWithTimeoutText =
            "Sorry, the search has timed out before it returned the results. The error has been logged and will be investigated.";

        public const string SearchStatsText = "Search took {0} and completed at {1}";

        public const string SearchStatsCountText = "{0} free slot{1} found";
        public const string SearchStatsPastCountText = "{0} past slot{1} found";
        public const string SearchAtDate = "As of {0}";

        public const string SearchDetailsButtonText = "Details";
        public const string SearchDetailsLabel = "Details:";
        public const string BackToResultsSummaryLabel = "Back to results summary";

        public const string ProviderOdsCodeRequiredErrorMessage = "You must enter a provider ODS code";
        public const string ConsumerOdsCodeRequiredErrorMessage = "You must enter a consumer ODS code";

        public const string ConsumerOrgTypeNotEnteredErrorMessage =
            "You have not selected a consumer organisation type. You must enter a consumer ODS code";

        public const string ConsumerOdsCodeNotEnteredErrorMessage =
            "You have not entered a consumer ODS code. You must select a consumer organisation type";

        public const string ConsumerInputRequiredErrorMessage =
            "You must enter a consumer ODS code OR select a consumer organisation type";

        public const string JobRoleRequiredErrorMessage = "You must enter a job role";
        public const string OrganisationRequiredErrorMessage = "You must enter an organisation";
        public const string AccessRequestReasonRequiredErrorMessage = "You must enter a reason";

        public const string ConsumerOdsCodeMaxLengthErrorMessage =
            "You have exceeded the maximum number of consumer ODS codes which you can enter ({0})";

        public const string ProviderOdsCodeMaxLengthErrorMessage =
            "You have exceeded the maximum number of provider ODS codes which you can enter ({0})";

        public const string ProviderOdsCodeMaxLengthMultiSearchNotEnabledErrorMessage =
            "You cannot enter multiple provider ODS codes because multi search is not enabled";

        public const string ConsumerOdsCodeMaxLengthMultiSearchNotEnabledErrorMessage =
            "You cannot enter multiple consumer ODS codes because multi search is not enabled";

        public const string ProviderOdsCodeRepeatedCodeErrorMessage =
            "Please remove the following repeated provider codes: {0}";

        public const string ConsumerOdsCodeRepeatedCodErrorMessage =
            "Please remove the following repeated consumer codes: {0}";

        public const string ProviderOdsCodeValidErrorMessage = "You must enter a valid provider ODS code";
        public const string ConsumerOdsCodeValidErrorMessage = "You must enter a valid consumer ODS code";
        public const string UserEmailAddressValidErrorMessage = "You must enter a valid nhs.net email address";
        public const string EmailAddressRequiredErrorMessage = "You must enter an nhs.net email address";

        public const string AddNewUserText = "Add a new user";
        public const string NewUserEmailAddress = AddNewUserText + ". Enter an nhs.net email address";
        public const string SaveNewUserButtonText = "Save";
        public const string ExportSearchResultsButtonText = "Export results";
        public const string UserListText = "User list";

        public const string SubmitButtonText = "Submit";
        public const string ContinueButtonText = "Continue";
        public const string CancelButtonText = "Cancel";
        public const string ClearButtonText = "Clear";
        public const string RefreshButtonText = "Refresh";
        public const string RegisterButtonText = "Start registration";

        public const string SearchResultsSearchAtHeadingText = "Search at";
        public const string SearchResultsSearchOnBehalfOfHeadingText = "Search on behalf of";

        public const string SearchResultsSearchAtText = "Searching at:";
        public const string SearchResultsPublisherLabel = "Provider system:";
        public const string SearchResultsSearchOnBehalfOfText = "Searching on behalf of:";
        public const string SearchResultsSearchOnBehalfOfConsumerOrgTypeText = "Searching with organisation type:";
        public const string SearchResultsSearchOnbehalfOfOrgTypeText = "Organisation type:";
        public const string SearchResultsAvailableAppointmentSlotsText = "Available Appointment Slots";
        public const string SearchResultsNoAvailableAppointmentSlotsText = "No available appointment slots found";
        public const string SearchResultsNoAddressProvidedText = "(no address provided)";
        public const string SearchResultsPastSlotsText = "Past Appointment Slots";

        public const string SearchResultsAppointmentDateColumnheaderText = "Appointment date";
        public const string SearchResultsSessionNameColumnHeaderText = "Session name";
        public const string SearchResultsStartTimeColumnHeaderText = "Start time";
        public const string SearchResultsDurationColumnHeaderText = "Duration";
        public const string SearchResultsSlotTypeColumnHeaderText = "Slot type";
        public const string SearchResultsModeOfAppointmentColumnheaderText = "Mode of appointment";
        public const string SearchResultsPractitionerColumnHeaderText = "Practitioner";

        public const string UserListResultsEmailAddress = "Email";
        public const string UserListResultsDisplayName = "Name (Organisation)";
        public const string UserListResultsStatus = "Status";
        public const string UserListResultsAccessLevel = "Access Level";
        public const string UserListResultsLastLogonDate = "Last signed on";
        public const string UserListResultsMultiSearchEnabled = "Multi Search?";
        public const string UserListResultsAccessRequestCount = "Number of access requests";
        public const string UserListResultsOrgTypeSearchEnabled = "Org Type Search?";
        public const string UserListResultsIsAdmin = "Is Admin?";

        public const string SearchInputProviderOdsCodeLabel = "Enter a provider ODS code";
        public const string SearchInputConsumerOdsCodeLabel = "Enter a consumer ODS code";
        public const string SearchInputConsumerOrganisationTypeLabel = "Select a consumer organisation type";

        public const string SearchInputProviderOdsCodeMultiLabel = "Enter one or more provider ODS codes";
        public const string SearchInputConsumerOdsCodeMultiLabel = "Enter one or more consumer ODS codes";

        public const string SearchInputConsumerMultiLabel =
            "Enter one or more consumer ODS codes, or select a consumer organisation type, or both";

        public const string SearchByConsumerOdsCodeText = "Search by consumer ODS code";
        public const string SearchByConsumerOrganisationTypeText = "Search by consumer organisation type";

        public const string SearchInputProviderOdsCodeHintText =
            "Enter up to {0} codes, separated by a space or a comma. If you enter more than one, you can only enter one consumer ODS code in '" +
            SearchResultsSearchOnBehalfOfHeadingText + "' below.";

        public const string SearchInputConsumerOdsCodeHintText =
            "Enter up to {0} codes, separated by a space or a comma. If you enter more than one, you can only enter one provider ODS code in '" +
            SearchResultsSearchAtHeadingText + "' above.";

        public const string SearchInputMustEnterConsumerOrgTypeHintText =
            "Enter a consumer ODS code, or select a consumer organisation type, or both";

        public const string SearchInputConsumerOrganisationTypeHintText =
            "Select a consumer organisation type or enter a consumer ODS code above, or both";

        public const string SearchInputDateRangeLabel = "Select a date range";
        public const string SearchForFreeSlotsButtonText = "Search for free slots";
        public const string UsersFoundText = "{0} user(s) found";

        public const string OdsLookup =
            "If you don't know the ODS code for the organisation, <a href='{0}' target='_blank'>click here to find it</a>";
    }
}