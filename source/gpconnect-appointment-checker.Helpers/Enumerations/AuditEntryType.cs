namespace gpconnect_appointment_checker.Helpers.Enumerations
{
    public enum AuditEntryType
    {
        UserLogonSuccess = 1,
        UserLogonFailed = 2,
        UserLogoff = 3,
        SlotSearch = 4,
        UserDisplayNameChanged = 5,
        UserOrganisationChanged = 6,
        OrganisationNameChanged = 7,
        OrganisationTypeChanged = 8,
        OrganisationAddressChanged = 9,
        UserStatusChanged = 10,
        UserAccessLevelChanged = 11,
        UserMultiSearchEnabledStatusChanged = 12,
        NewUserAdded = 13,
        EmailSent = 14
    }
}
