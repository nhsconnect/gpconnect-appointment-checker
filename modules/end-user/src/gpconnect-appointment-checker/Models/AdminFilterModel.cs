using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;

namespace gpconnect_appointment_checker.Models;

public class AdminFilterModel
{
    [BindProperty(SupportsGet = true)] public string? Surname { get; set; }
    [BindProperty(SupportsGet = true)] public string? EmailAddress { get; set; }
    [BindProperty(SupportsGet = true)] public UserAccountStatus? UserAccountStatusFilter { get; set; }
    [BindProperty(SupportsGet = true)] public AccessLevel? AccessLevelFilter { get; set; }
    [BindProperty(SupportsGet = true)] public bool? MultiSearchFilter { get; set; }
    [BindProperty(SupportsGet = true)] public bool? OrgTypeSearchFilter { get; set; }
}