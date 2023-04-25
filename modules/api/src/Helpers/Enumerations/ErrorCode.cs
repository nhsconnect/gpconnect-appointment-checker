using System.ComponentModel;

namespace GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

public enum ErrorCode : ushort
{
    [Description("")]
    None = 0,
    [Description(Constants.SearchConstants.ISSUEWITHPROVIDERODSCODETEXT)] 
    ProviderODSCodeNotFound = 1,
    [Description(Constants.SearchConstants.ISSUEWITHCONSUMERODSCODETEXT)]
    ConsumerODSCodeNotFound = 2,
    [Description(Constants.SearchConstants.ISSUEWITHGPCONNECTPROVIDERTEXT)] 
    ProviderASIDCodeNotFound = 3,
    [Description(Constants.SearchConstants.ISSUEWITHGPCONNECTPROVIDERNOTENABLEDTEXT)]
    ProviderNotEnabledForGpConnectAppointmentManagement = 4,
    [Description(Constants.SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT)]
    CapabilityStatementNotFound = 5,
    [Description(Constants.SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT)]
    CapabilityStatementHasErrors = 6,
    [Description(Constants.SearchConstants.ISSUEWITHLDAPTEXT)]
    GenericLdapError = 7,
    GenericSlotSearchError = 8,
    [Description(Constants.SearchConstants.ISSUEWITHGPCONNECTCONSUMERNOTENABLEDTEXT)]
    ConsumerNotEnabledForGpConnectAppointmentManagement = 9
}
