using gpconnect_appointment_checker.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserListAdvanced
{
    public string? Surname { get; set; }
    public string? EmailAddress { get; set; }
    public string? OrganisationName { get; set; }
    public UserAccountStatus? UserAccountStatusFilter { get; set; }
    public AccessLevel? AccessLevelFilter { get; set; }
    public bool? MultiSearchFilter { get; set; }
    public bool? OrgTypeSearchFilter { get; set; }
    public SortBy SortByColumn { get; set; } = SortBy.EmailAddress;
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}
