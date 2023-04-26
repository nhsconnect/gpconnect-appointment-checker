using gpconnect_appointment_checker.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Models.Request;

public class UserListSimple
{
    public SortBy SortByColumn { get; set; } = SortBy.EmailAddress;
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}
