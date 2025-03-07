namespace gpconnect_appointment_checker.Helpers.Constants
{
    public class SearchConstants
    {
        public const string Issuewithodscodeslabel = "Issue with ODS codes";
        public const string Issuewithconsumerodscodelabel = "Issue with Consumer ODS code";
        public const string Issuewithproviderodscodelabel = "Issue with Provider ODS code";
        public const string Issuewithgpconnectproviderlabel = "Issue with GP Connect Provider";
        public const string Issuewithgpconnectconsumerlabel = "Issue with GP Connect Consumer";
        public const string Issuewithsendingmessagetoprovidersystemlabel = "An error occured when sending a message to the provider system";
        public const string Issuewithldaplabel = "An LDAP error has occurred";
        public const string Multiplecodehinttext = "Please be aware that the search may take a while if multiple codes have been entered.";

        public const string Accesslabel = "Access to GP Connect Appointment Checker";
        public const string Accesstext = "You must be authorised to use this application. Please contact <a href=\"mailto:{0}?subject=Appointment%20checker%20user%20auth%20request\">{0}</a> to request access, providing your name, role and organisation.";
        public const string Accessgrantedtext = "Once you have been granted access, please click <a href=\"{0}\">here</a> to return to the login page.";
        public const string Accesstextalt = "This application is restricted to registered users of the NHS. Please sign in to use it.";
        public const string Accesstextregisteralt = "If you don't have an account, please <a href=\"{0}\">" + Createaccountbuttontext + "</a>.";

        public const string Notregisteredtext = "You are not registered to use the Appointment Checker.";
        public const string Notregisteredlinktext = "Please <a href=\"{0}\">" + Createaccountbuttontext + "</a> for an account.";
        
        public const string Createaccountinterstitiallabel = "Register for GP Connect Appointment Checker";
        public const string Createaccountinterstitialtext = "<p>Your NHS Mail email address is needed to register to use the GP Connect Appointment Checker.</p><p>Please sign in with your NHS Mail account to start the registration process.</p>";
        public const string Createaccountinterstitialalttext = "Or <a href=\"{0}\">Cancel</a> to return to the home page.";

        public const string Createaccountcanceltext = "If you do not wish to continue, please <a href=\"{0}\">" + Canceltext + "</a> to return to the home page.";
        
        public const string Usernamelabel = "User name";
        public const string Jobrolelabel = "Job role";
        public const string Organisationlabel = "Organisation (if different to the one shown above)";
        public const string Accessrequestreasonlabel = "Reason for requesting access to the Appointment Checker.";
        public const string Accessrequestreasonhinttext = "Please provide as much information as possible, this will allow your account to be created quickly. For example, \"I am responsible for setting up slots across my PCN and want to check them.\"";

        public const string Signedintext = "Signed in to GP Connect Appointment Checker as {0}";
        public const string Runasearchtext = "Run a search";
        public const string Alreadyhaveanaccounttext = "You already have an account and can use the Appointment Checker";
        public const string Searchtext = "Search";
        public const string Admintext = "Admin";
        public const string Reportstext = "Reports";
        public const string Hometext = "Home";
        public const string Helptext = "Help";
        public const string Signinbuttontext = "Sign in";
        public const string Createaccountbuttontext = "Register";
        public const string Canceltext = "Cancel";
        public const string Registertousetext = "Register to use the Appointment Checker";

        public const string Pendingaccounttext = "You have previously asked for an account to be created, and the request is pending. You will receive an email when the request has been authorised.";
                
        public const string Accountdisabledlabel = "Account Disabled";
        public const string Requestsubmittedlabel = "Request Submitted";
        public const string Requestsubmittedtext = "Your request to create an account has been submitted. You will receive an email as soon as your account has been set up.";
        public const string Requestsubmittederrortext = "There was a problem creating your Appointment Checker account. Please email <a href=\"mailto:{0}\">{0}</a> and your account will be set up manually.";

        public const string Submituserformlabel = "Access Denied";
        public const string Submituserformtext = "You do not have the necessary authorisation to use the Appointment Checker, or a previous request was denied. You need to complete a short form and confirm that you have agreed to the Terms and Conditions. Click on the button below to continue.";
        
        public const string Signoutbuttontext = "Sign out";
        public const string Deauthorisebuttontext = "De-authorise";
        public const string Authorisebuttontext = "Authorise";

        public const string Applyfilterbuttontext = "Apply filters";

        public const string Privacyandcookiestext = "Privacy and Cookies";
        public const string Accessibilitystatementtext = "Accessibility Statement";
        public const string Termsandconditionstext = "Terms and Conditions";

        public const string Issuewithodscodesinputlabel = "Issue with ODS codes entered";
        public const string Issuewithodscodesinputtext = "Sorry, but you cannot enter multiple Provider ODS codes and multiple Consumer ODS codes at the same time.";

        public const string Issuewithodscodestext = "Sorry, but the Provider ODS code '{0}' and the Consumer ODS code '{1}' have not been found";
        public const string Issuewithconsumerodscodetext = "Sorry, but the Consumer ODS code '{0}' has not been found";
        public const string Issuewithproviderodscodetext = "Sorry, but the Provider ODS code '{0}' has not been found";
        public const string Issuewithgpconnectprovidertext = "Sorry, this organisation is not enabled for GP Connect";
        public const string Issuewithgpconnectprovidernotenabledtext = "Sorry, but the Provider ODS code {0} is not enabled for GP Connect Appointment Management";
        public const string Issuewithgpconnectconsumernotenabledtext = "The Consumer ODS code {0} is not enabled for GP Connect";

        public const string Issuewithsendingmessagetoprovidersystemtext = "The error returned was \"{0} ({1})\".";
        public const string Issuewithldaptext = "The SDS server is currently unavailable. Please try your search again.";

        public const string Errorheadertitletext = "Error in GP Connect Appointment Checker";
        public const string Statuscodeerrortitletext = "Status Code Error in GP Connect Appointment Checker";
        public const string Errortext = "Sorry, but it looks like an error has occurred in the application. The error has been logged and will be investigated.";
        public const string Returntohomepagetext = "Please click here to return to the Home page.";
        public const string Returntosearchpagetext = "Please click here to return to the Search page.";

        public const string Issuewithtimeouttitletext = "Issue with timeout";
        public const string Issuewithtimeouttext = "Sorry, the search has timed out before it returned the results. The error has been logged and will be investigated.";
        
        public const string Searchstatstext = "Search took {0} and completed at {1}";

        public const string Searchstatscounttext = "{0} free slot{1} found";
        public const string Searchstatspastcounttext = "{0} past slot{1} found";

        public const string Searchdetailsbuttontext = "Details";
        public const string Searchdetailslabel = "Details:";
        public const string Backtoresultssummarylabel = "Back to results summary";

        public const string Providerodscoderequirederrormessage = "You must enter a provider ODS code";
        public const string Consumerodscoderequirederrormessage = "You must enter a consumer ODS code";

        public const string Consumerorgtypenotenterederrormessage = "You have not selected a consumer organisation type. You must enter a consumer ODS code";
        public const string Consumerodscodenotenterederrormessage = "You have not entered a consumer ODS code. You must select a consumer organisation type";

        public const string Consumerinputrequirederrormessage = "You must enter a consumer ODS code OR select a consumer organisation type";
        public const string Jobrolerequirederrormessage = "You must enter a job role";
        public const string Organisationrequirederrormessage = "You must enter an organisation";
        public const string Accessrequestreasonrequirederrormessage = "You must enter a reason";

        public const string Consumerodscodemaxlengtherrormessage = "You have exceeded the maximum number of consumer ODS codes which you can enter ({0})";
        public const string Providerodscodemaxlengtherrormessage = "You have exceeded the maximum number of provider ODS codes which you can enter ({0})";
        public const string Providerodscodemaxlengthmultisearchnotenablederrormessage = "You cannot enter multiple provider ODS codes because multi search is not enabled";
        public const string Consumerodscodemaxlengthmultisearchnotenablederrormessage = "You cannot enter multiple consumer ODS codes because multi search is not enabled";

        public const string Providerodscoderepeatedcoderrormessage = "Please remove the following repeated provider codes: {0}";
        public const string Consumerodscoderepeatedcoderrormessage = "Please remove the following repeated consumer codes: {0}";

        public const string Providerodscodevaliderrormessage = "You must enter a valid provider ODS code";
        public const string Consumerodscodevaliderrormessage = "You must enter a valid consumer ODS code";
        public const string Useremailaddressvaliderrormessage = "You must enter a valid nhs.net email address";
        public const string Emailaddressrequirederrormessage = "You must enter an nhs.net email address";

        public const string Addnewusertext = "Add a new user";
        public const string Newuseremailaddress = Addnewusertext + ". Enter an nhs.net email address";
        public const string Savenewuserbuttontext = "Save";
        public const string Exportsearchresultsbuttontext = "Export results";
        public const string Userlisttext = "User list";

        public const string Submitbuttontext = "Submit";
        public const string Continuebuttontext = "Continue";
        public const string Cancelbuttontext = "Cancel";
        public const string Clearbuttontext = "Clear";
        public const string Registerbuttontext = "Start registration";

        public const string Searchresultssearchatheadingtext = "Search at";
        public const string Searchresultssearchonbehalfofheadingtext = "Search on behalf of";

        public const string Searchresultssearchattext = "Searching at:";
        public const string Searchresultspublisherlabel = "Provider system:";
        public const string Searchresultssearchonbehalfoftext = "Searching on behalf of:";
        public const string Searchresultssearchonbehalfofconsumerorgtypetext = "Searching with organisation type:";
        public const string Searchresultssearchonbehalfoforgtypetext = "Organisation type:";
        public const string Searchresultsavailableappointmentslotstext = "Available Appointment Slots";
        public const string Searchresultsnoavailableappointmentslotstext = "No available appointment slots found";
        public const string Searchresultsnoaddressprovidedtext = "(no address provided)";
        public const string Searchresultspastslotstext = "Past Appointment Slots";

        public const string Searchresultsappointmentdatecolumnheadertext = "Appointment date";
        public const string Searchresultssessionnamecolumnheadertext = "Session name";
        public const string Searchresultsstarttimecolumnheadertext = "Start time";
        public const string Searchresultsdurationcolumnheadertext = "Duration";
        public const string Searchresultsslottypecolumnheadertext = "Slot type";
        public const string Searchresultsmodeofappointmentcolumnheadertext = "Mode of appointment";
        public const string Searchresultspractitionercolumnheadertext = "Practitioner";

        public const string Userlistresultsemailaddress = "Email";
        public const string Userlistresultsdisplayname = "Name (Organisation)";
        public const string Userlistresultsstatus = "Status";
        public const string Userlistresultsaccesslevel = "Access Level";
        public const string Userlistresultslastlogondate = "Last signed on";
        public const string Userlistresultsmultisearchenabled = "Multi Search?";
        public const string Userlistresultsaccessrequestcount = "Number of access requests";
        public const string Userlistresultsorgtypesearchenabled = "Org Type Search?";
        public const string Userlistresultsisadmin = "Is Admin?";

        public const string Searchinputproviderodscodelabel = "Enter a provider ODS code";
        public const string Searchinputconsumerodscodelabel = "Enter a consumer ODS code";
        public const string Searchinputconsumerorganisationtypelabel = "Select a consumer organisation type";

        public const string Searchinputproviderodscodemultilabel = "Enter one or more provider ODS codes";
        public const string Searchinputconsumerodscodemultilabel = "Enter one or more consumer ODS codes";
        public const string Searchinputconsumermultilabel = "Enter one or more consumer ODS codes, or select a consumer organisation type, or both";

        public const string Searchbyconsumerodscodetext = "Search by consumer ODS code";
        public const string Searchbyconsumerorganisationtypetext = "Search by consumer organisation type";

        public const string Searchinputproviderodscodehinttext = "Enter up to {0} codes, separated by a space or a comma. If you enter more than one, you can only enter one consumer ODS code in '" + Searchresultssearchonbehalfofheadingtext + "' below.";
        public const string Searchinputconsumerrodscodehinttext = "Enter up to {0} codes, separated by a space or a comma. If you enter more than one, you can only enter one provider ODS code in '" + Searchresultssearchatheadingtext + "' above.";

        public const string Searchinputmustenterconsumerorgtypehinttext = "Enter a consumer ODS code, or select a consumer organisation type, or both";

        public const string Searchinputconsumerorganisationtypehinttext = "Select a consumer organisation type or enter a consumer ODS code above, or both";

        public const string Searchinputdaterangelabel = "Select a date range";
        public const string Searchforfreeslotsbuttontext = "Search for free slots";
        public const string Usersfoundtext = "{0} user(s) found";
    }
}
