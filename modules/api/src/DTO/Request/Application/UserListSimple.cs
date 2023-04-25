using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;
using Microsoft.AspNetCore.Mvc;

namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class UserListSimple
{
    [BindProperty(Name = "sort_by_column", SupportsGet = true)]
    public SortBy SortByColumn { get; set; } = SortBy.EmailAddress;
    [BindProperty(Name = "sort_direction", SupportsGet = true)]
    public SortDirection SortDirection { get; set; } = SortDirection.ASC;
}

