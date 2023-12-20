namespace GpConnect.AppointmentChecker.Api.DTO.Request.Application;
public class SearchResult
{
    public int SearchGroupId { get; set; }
    public string ProviderCode { get; set; }
    public string ConsumerCode { get; set; }
    public int ErrorCode { get; set; }
    public List<string> Details { get; set; }
    public string ProviderPublisher { get; set; }
    public int? SpineMessageId { get; set; }
    public double SearchDurationSeconds { get; set; }
    public string ConsumerOrganisationType { get; set; }
}
