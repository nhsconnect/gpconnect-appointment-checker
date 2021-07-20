namespace gpconnect_appointment_checker.Helpers.Constants
{
    public class SearchConstants
    {
        public const string ISSUEWITHODSCODESLABEL = "Issue with ODS codes";
        public const string ISSUEWITHCONSUMERODSCODELABEL = "Issue with Consumer ODS code";
        public const string ISSUEWITHPROVIDERODSCODELABEL = "Issue with Provider ODS code";
        public const string ISSUEWITHGPCONNECTPROVIDERLABEL = "Issue with GP Connect Provider";
        public const string ISSUEWITHGPCONNECTCONSUMERLABEL = "Issue with GP Connect Consumer";
        public const string ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMLABEL = "An error occured when sending a message to the provider system";
        public const string ISSUEWITHLDAPLABEL = "An LDAP error has occurred";
        public const string MULTIPLECODEHINTTEXT = "Please be aware that the search may take a while if multiple codes have been entered.";

        public const string ACCESSLABEL = "Access to the GP Connect Appointment Checker";
        public const string ACCESSTEXT = "You must be authorised to use this application. Please contact <a href=\"mailto:{0}?subject=Appointment%20checker%20user%20auth%20request\">{0}</a> to request access, providing your name, role and organisation.";
        public const string ACCESSGRANTEDTEXT = "Once you have been granted access, please click <a href=\"{0}\">here</a> to return to the login page.";
        public const string ACCESSTEXTALT = "This application is restricted to registered users of the NHS. Please sign in to use it.";
        public const string ACCESSTEXTREGISTERALT = "If you don't have an account, please <a href=\"{0}\">" + CREATEACCOUNTBUTTONTEXT + "</a>.";

        public const string NOTREGISTEREDTEXT = "You are not registered to use the Appointment Checker.";
        public const string NOTREGISTEREDLINKTEXT = "Please <a href=\"{0}\">" + CREATEACCOUNTBUTTONTEXT + "</a> for an account.";
        
        public const string CREATEACCOUNTINTERSTITIALLABEL = "Register to use the GP Connect Appointment Checker";
        public const string CREATEACCOUNTINTERSTITIALTEXT = "<p>Your NHS Mail email address is needed to register to use the GP Connect Appointment Checker.</p><p>Please sign in with your NHS Mail account to start the registration process.</p>";
        public const string CREATEACCOUNTINTERSTITIALALTTEXT = "Or <a href=\"{0}\">Cancel</a> to return to the home page.";

        public const string CREATEACCOUNTCANCELTEXT = "If you do not wish to continue, please <a href=\"{0}\">" + CANCELTEXT + "</a> to return to the home page.";
        
        public const string USERNAMELABEL = "User name";
        public const string JOBROLELABEL = "Job role";
        public const string ORGANISATIONLABEL = "Organisation (if different to the one shown above)";
        public const string ACCESSREQUESTREASONLABEL = "Reason for requesting access to the Appointment Checker.";
        public const string ACCESSREQUESTREASONHINTTEXT = "Please provide as much information as possible, this will allow your account to be created quickly. For example, \"I am responsible for setting up slots acrosss my PCN and want to check them.\"";

        public const string SIGNEDINTEXT = "Signed in to GP Connect Appointment Checker as<div>{0}</div>";
        public const string RUNASEARCHTEXT = "Run a search";
        public const string ALREADYHAVEANACCOUNTTEXT = "You already have an account and can use the Appointment Checker";
        public const string SEARCHTEXT = "Search";
        public const string ADMINTEXT = "Admin";
        public const string REPORTSTEXT = "Reports";
        public const string HOMETEXT = "Home";
        public const string HELPTEXT = "Help";
        public const string SIGNINBUTTONTEXT = "Sign in";
        public const string CREATEACCOUNTBUTTONTEXT = "Register";
        public const string CANCELTEXT = "Cancel";
        public const string REGISTERTOUSETEXT = "Register to use the Appointment Checker";

        public const string PENDINGACCOUNTTEXT = "You have previously asked for an account to be created, and the request is pending. You will receive an email when the request has been authorised.";
                
        public const string ACCOUNTDISABLEDLABEL = "Account Disabled";
        public const string REQUESTSUBMITTEDLABEL = "Request Submitted";
        public const string REQUESTSUBMITTEDTEXT = "Your request to create an account has been submitted. You will receive an email as soon as your account has been set up.";
        public const string REQUESTSUBMITTEDERRORTEXT = "There was a problem creating your Appointment Checker account. Please email <a href=\"mailto:{0}\">{0}</a> and your account will be set up manually.";

        public const string SUBMITUSERFORMLABEL = "Access Denied";
        public const string SUBMITUSERFORMTEXT = "You do not have the necessary authorisation to use the Appointment Checker, or a previous request was denied. You need to complete a short form and confirm that you have agreed to the Terms and Conditions. Click on the button below to continue.";
        
        public const string SIGNOUTBUTTONTEXT = "Sign out";
        public const string DEAUTHORISEBUTTONTEXT = "De-authorise";
        public const string AUTHORISEBUTTONTEXT = "Authorise";

        public const string FINDBUTTONTEXT = "Find";
        public const string FILTERBYSTATUSBUTTONTEXT = "Status Filter";

        public const string PRIVACYANDCOOKIESTEXT = "Privacy and Cookies";
        public const string ACCESSIBILITYSTATEMENTTEXT = "Accessibility Statement";
        public const string TERMSANDCONDITIONSTEXT = "Terms and Conditions";

        public const string ISSUEWITHODSCODESINPUTLABEL = "Issue with ODS codes entered";
        public const string ISSUEWITHODSCODESINPUTTEXT = "Sorry, but you cannot enter multiple Provider ODS codes and multiple Consumer ODS codes at the same time.";

        public const string ISSUEWITHODSCODESTEXT = "Sorry, but the Provider ODS code '{0}' and the Consumer ODS code '{1}' have not been found";
        public const string ISSUEWITHCONSUMERODSCODETEXT = "Sorry, but the Consumer ODS code '{0}' has not been found";
        public const string ISSUEWITHPROVIDERODSCODETEXT = "Sorry, but the Provider ODS code '{0}' has not been found";
        public const string ISSUEWITHGPCONNECTPROVIDERTEXT = "Sorry, this organisation is not enabled for GP Connect";
        public const string ISSUEWITHGPCONNECTPROVIDERNOTENABLEDTEXT = "Sorry, but the Provider ODS code {0} is not enabled for GP Connect Appointment Management";
        public const string ISSUEWITHGPCONNECTCONSUMERNOTENABLEDTEXT = "The Consumer ODS code {0} is not enabled for GP Connect";

        public const string ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT = "The error returned was \"{0} ({1})\".";
        public const string ISSUEWITHLDAPTEXT = "The SDS server is currently unavailable. Please try your search again.";

        public const string ERRORHEADERTITLETEXT = "Error in GP Connect Appointment Checker";
        public const string ERRORTEXT = "Sorry, but it looks like an error has occurred in the application. The error has been logged and will be investigated.";
        public const string RETURNTOHOMEPAGETEXT = "Please click here to return to the Home page.";
        public const string RETURNTOSEARCHPAGETEXT = "Please click here to return to the Search page.";

        public const string ISSUEWITHTIMEOUTTITLETEXT = "Issue with timeout";
        public const string ISSUEWITHTIMEOUTTEXT = "Sorry, the search has timed out before it returned the results. The error has been logged and will be investigated.";
        
        public const string SEARCHSTATSTEXT = "Search took {0} and completed at {1}<br />{2} free slots found";

        public const string SEARCHDETAILSBUTTONTEXT = "Details";
        public const string SEARCHDETAILSLABEL = "Details:";
        public const string BACKTORESULTSSUMMARYLABEL = "Back to results summary";

        public const string PROVIDERODSCODEREQUIREDERRORMESSAGE = "You must enter a provider ODS code";
        public const string CONSUMERODSCODEREQUIREDERRORMESSAGE = "You must enter a consumer ODS code";
        public const string JOBROLEREQUIREDERRORMESSAGE = "You must enter a job role";
        public const string ORGANISATIONREQUIREDERRORMESSAGE = "You must enter an organisation";
        public const string ACCESSREQUESTREASONREQUIREDERRORMESSAGE = "You must enter a reason";

        public const string CONSUMERODSCODEMAXLENGTHERRORMESSAGE = "You have exceeded the maximum number of consumer ODS codes which you can enter ({0})";
        public const string PROVIDERODSCODEMAXLENGTHERRORMESSAGE = "You have exceeded the maximum number of provider ODS codes which you can enter ({0})";
        public const string PROVIDERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE = "You cannot enter multiple provider ODS codes because multi search is not enabled";
        public const string CONSUMERODSCODEMAXLENGTHMULTISEARCHNOTENABLEDERRORMESSAGE = "You cannot enter multiple consumer ODS codes because multi search is not enabled";

        public const string PROVIDERODSCODEREPEATEDCODERRORMESSAGE = "Please remove the following repeated provider codes: {0}";
        public const string CONSUMERODSCODEREPEATEDCODERRORMESSAGE = "Please remove the following repeated consumer codes: {0}";

        public const string PROVIDERODSCODEVALIDERRORMESSAGE = "You must enter a valid provider ODS code";
        public const string CONSUMERODSCODEVALIDERRORMESSAGE = "You must enter a valid consumer ODS code";
        public const string USEREMAILADDRESSVALIDERRORMESSAGE = "You must enter a valid nhs.net email address";
        public const string EMAILADDRESSREQUIREDERRORMESSAGE = "You must enter an nhs.net email address";

        public const string ADDNEWUSERTEXT = "Add a new user";
        public const string NEWUSEREMAILADDRESS = ADDNEWUSERTEXT + ". Enter an nhs.net email address";
        public const string SAVENEWUSERBUTTONTEXT = "Save";
        public const string LOADREPORTBUTTONTEXT = "Load Report";
        public const string EXPORTREPORTBUTTONTEXT = "Export Report";
        public const string USERLISTTEXT = "User list";

        public const string SUBMITBUTTONTEXT = "Submit";
        public const string CONTINUEBUTTONTEXT = "Continue";
        public const string CANCELBUTTONTEXT = "Cancel";
        public const string CLEARBUTTONTEXT = "Clear";
        public const string REGISTERBUTTONTEXT = "Register";

        public const string SEARCHRESULTSSEARCHATTEXT = "Searching at:";
        public const string SEARCHRESULTSPUBLISHERLABEL = "Provider system:";
        public const string SEARCHRESULTSSEARCHONBEHALFOFTEXT = "Searching on behalf of:";
        public const string SEARCHRESULTSAVAILABLEAPPOINTMENTSLOTSTEXT = "Available Appointment Slots";
        public const string SEARCHRESULTSNOAVAILABLEAPPOINTMENTSLOTSTEXT = "No available appointment slots found";
        public const string SEARCHRESULTSNOADDRESSPROVIDEDTEXT = "(no address provided)";

        public const string SEARCHRESULTSAPPOINTMENTDATECOLUMNHEADERTEXT = "Appointment date";
        public const string SEARCHRESULTSSESSIONNAMECOLUMNHEADERTEXT = "Session name";
        public const string SEARCHRESULTSSTARTTIMECOLUMNHEADERTEXT = "Start time";
        public const string SEARCHRESULTSDURATIONCOLUMNHEADERTEXT = "Duration";
        public const string SEARCHRESULTSSLOTTYPECOLUMNHEADERTEXT = "Slot type";
        public const string SEARCHRESULTSMODEOFAPPOINTMENTCOLUMNHEADERTEXT = "Mode of appointment";
        public const string SEARCHRESULTSPRACTITIONERCOLUMNHEADERTEXT = "Practitioner";

        public const string USERLISTRESULTSEMAILADDRESS = "Email";
        public const string USERLISTRESULTSDISPLAYNAME = "Name";
        public const string USERLISTRESULTSORGANISATIONNAME = "Organisation";
        public const string USERLISTRESULTSSTATUS = "Status";
        public const string USERLISTRESULTSACCESSLEVEL = "Access Level";
        public const string USERLISTRESULTSLASTLOGONDATE = "Last signed on";
        public const string USERLISTRESULTSMULTISEARCHENABLED = "Multi Search?";
        public const string USERLISTRESULTSACCESSREQUESTCOUNT = "Number of access requests";

        public const string SEARCHINPUTPROVIDERODSCODELABEL = "Enter a provider ODS code (search at)";
        public const string SEARCHINPUTCONSUMERODSCODELABEL = "Enter a consumer ODS code (search on behalf of)";

        public const string SEARCHINPUTPROVIDERODSCODEMULTILABEL = "Enter one or more provider ODS codes (search at)";
        public const string SEARCHINPUTCONSUMERODSCODEMULTILABEL = "Enter one or more consumer ODS codes (search on behalf of)";

        public const string SEARCHINPUTPROVIDERODSCODEHINTTEXT = "Enter up to {0} codes, separated by a space or a comma.<br/>If you enter more than one, you can only enter one consumer ODS code below.";
        public const string SEARCHINPUTCONSUMERRODSCODEHINTTEXT = "Enter up to {0} codes, separated by a space or a comma.<br/>If you enter more than one, you can only enter one provider ODS code above.";

        public const string SEARCHINPUTDATERANGELABEL = "Select a date range";
        public const string SEARCHFORFREESLOTSBUTTONTEXT = "Search for free slots";
        public const string FREESLOTSFOUNDTEXT = "{0} free slots found";
    }
}
