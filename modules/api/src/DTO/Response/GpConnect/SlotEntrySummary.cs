namespace GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

public class SlotEntrySummary
{
    public string ProviderLocationName { get; set; }
    public string ProviderOdsCode { get; set; }
    public string ConsumerLocationName { get; set; }
    public string ConsumerOdsCode { get; set; }
    public string ConsumerOrganisationType { get; set; }
    public string SearchSummaryDetail { get; set; }
    public string SearchSummaryAdditionalDetail { get; set; }
    public string ProviderPublisher { get; set; }
    public int SearchResultId { get; set; }
    public int SearchGroupId { get; set; }
    public bool DetailsEnabled { get; set; }
    public bool DisplayProvider { get; set; }
    public bool DisplayConsumer { get; set; }
    public string DisplayClass { get; set; }
    public bool DisplayConsumerOrganisationType => !string.IsNullOrEmpty(ConsumerOrganisationType);
}
