﻿namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;

public class SearchGroup
{
    public int UserSessionId { get; set; }
    public string ProviderOdsTextbox { get; set; }
    public string ConsumerOdsTextbox { get; set; }
    public string SearchDateRange { get; set; }
    public string ConsumerOrganisationTypeDropdown { get; set; }
}
