using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserListAdvanced : BaseUserList
{
    [BindProperty(Name = "surname", SupportsGet = true)]
    public string? Surname { get; set; }
    [BindProperty(Name = "email_address", SupportsGet = true)]
    public string? EmailAddress { get; set; }
    [BindProperty(Name = "user_account_status_filter", SupportsGet = true)]
    public UserAccountStatus? UserAccountStatusFilter { get; set; }
    [BindProperty(Name = "access_level_filter", SupportsGet = true)]
    public AccessLevel? AccessLevelFilter { get; set; }
    [BindProperty(Name = "multi_search_filter", SupportsGet = true)]
    public bool? MultiSearchFilter { get; set; }
    [BindProperty(Name = "org_type_search_filter", SupportsGet = true)]
    public bool? OrgTypeSearchFilter { get; set; }    
}
