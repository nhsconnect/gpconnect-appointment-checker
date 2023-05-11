namespace GpConnect.AppointmentChecker.Api.Helpers.Constants;

public class SearchConstants
{
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

    public const string SEARCHRESULTSSEARCHONBEHALFOFORGTYPETEXT = "Organisation Type - {0}";

    public const string SEARCHSTATSCOUNTTEXT = "{0} free slot{1} found";
    public const string SEARCHSTATSPASTCOUNTTEXT = "{0} past slot{1} found";
    public const string SEARCHRESULTSNOAVAILABLEAPPOINTMENTSLOTSTEXT = "No available appointment slots found";
}
