using Newtonsoft.Json;
using System;

namespace GpConnect.AppointmentChecker.Models;

public class User
{
    [JsonProperty("userId")]
    public int UserId { get; set; }
    
    [JsonProperty("userSessionId")]
    public int UserSessionId { get; set; }

    [JsonProperty("emailAddress")]
    public string EmailAddress { get; set; }

    [JsonProperty("displayName")]
    public string DisplayName { get; set; }

    [JsonProperty("organisationName")]
    public string OrganisationName { get; set; }

    [JsonProperty("accessLevel")]
    public string AccessLevel { get; set; }

    [JsonProperty("lastLogonDate")]
    public DateTime? LastLogonDate { get; set; }

    [JsonProperty("userAccountStatusId")]
    public int UserAccountStatusId { get; set; }

    [JsonProperty("multiSearchEnabled")]
    public bool MultiSearchEnabled { get; set; }

    [JsonProperty("isAdmin")]
    public bool IsAdmin { get; set; }

    [JsonProperty("isNewUser")]
    public bool IsNewUser { get; set; }

    [JsonProperty("accessRequestCount")]
    public int AccessRequestCount { get; set; }

    [JsonProperty("isPastLastLogonThreshold")]
    public bool IsPastLastLogonThreshold { get; set; }

    [JsonProperty("statusChanged")]
    public bool StatusChanged { get; set; }

    [JsonProperty("organisationId")]
    public int OrganisationId { get; set; }

    [JsonProperty("orgTypeSearchEnabled")]
    public bool OrgTypeSearchEnabled { get; set; }
}
