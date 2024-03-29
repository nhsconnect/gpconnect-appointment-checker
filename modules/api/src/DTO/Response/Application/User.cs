﻿using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Enumerations;

namespace GpConnect.AppointmentChecker.Api.DTO.Response.Application;

public class User
{
    public int UserId { get; set; }
    public int UserSessionId { get; set; }
    public string EmailAddress { get; set; }
    public string DisplayName { get; set; }
    public string AccessLevel => IsAdmin ? "Admin" : "User";
    public DateTime? LastLogonDate { get; set; }
    public string LastLogonDateShort => LastLogonDate.HasValue ? LastLogonDate.Value.ToString("d MMM yyyy") : string.Empty;
    public int UserAccountStatusId { get; set; }
    public string UserAccountStatus => ((UserAccountStatus)UserAccountStatusId).GetDescription();
    public bool MultiSearchEnabled { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsNewUser { get; set; }
    public int AccessRequests { get; set; }
    public bool IsPastLastLogonThreshold { get; set; }
    public bool StatusChanged { get; set; }
    public int OrganisationId { get; set; }
    public bool OrgTypeSearchEnabled { get; set; }
    public bool IsRequestUser { get; set; }
}
