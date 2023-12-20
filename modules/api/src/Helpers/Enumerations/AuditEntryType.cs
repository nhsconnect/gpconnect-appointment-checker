namespace GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

public enum AuditEntryType
{
    UserLogonSuccess = 1,
    UserLogonFailed = 2,
    UserLogoff = 3,
    SingleSlotSearch = 4,
    UserDisplayNameChanged = 5,
    UserOrganisationChanged = 6,
    OrganisationNameChanged = 7,
    OrganisationTypeChanged = 8,
    OrganisationAddressChanged = 9,
    UserStatusChanged = 10,
    UserAccessLevelChanged = 11,
    UserMultiSearchEnabledStatusChanged = 12,
    NewUserAdded = 13,
    EmailSent = 14,
    MultiSlotSearch = 15,
    UserCreateAccount = 16,
    UserOrgTypeSearchEnabledStatusChanged = 17
}