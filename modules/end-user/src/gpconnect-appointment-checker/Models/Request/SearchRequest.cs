using System;

namespace GpConnect.AppointmentChecker.Models.Request;

public class SearchRequest
{
    public string ProviderOdsCode { get; set; }
    public string ConsumerOdsCode { get; set; }
    public string ConsumerOrganisationType { get; set; }
    public string DateRange { get; set; }
    public Uri RequestUri { get; set; }
    public int UserId { get; set; }
    public int UserSessionId { get; set; }
    public string Sid { get; set; }
}
