using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;

namespace GpConnect.AppointmentChecker.Api.DTO.Response;

public class SearchResponse
{
    public int SearchResultsTotalCount => SearchResultsCurrentCount + SearchResultsPastCount;
    public int SearchResultsCurrentCount { get; set; } = 0;
    public int SearchResultsPastCount { get; set; } = 0;

    public double TimeTaken { get; set; }

    public List<SlotEntrySimple> SearchResults { get; set; }
    public List<SlotEntrySimple> SearchResultsPast { get; set; }

    public List<List<SlotEntrySimple>> CurrentSlotEntriesByLocationGrouping => SearchResults?.GroupBy(l => l.LocationName).Select(grp => grp.ToList()).ToList();
    public List<List<SlotEntrySimple>> PastSlotEntriesByLocationGrouping => SearchResultsPast?.GroupBy(l => l.LocationName).Select(grp => grp.ToList()).ToList();

    public bool ProviderOdsCodeFound { get; set; } = false;
    public bool ConsumerOdsCodeFound { get; set; } = false;

    public string ProviderOdsCode { get; set; } = "";
    public string ConsumerOdsCode { get; set; } = "";

    public string FormattedProviderOrganisationDetails { get; set; } = "";
    public string FormattedConsumerOrganisationDetails { get; set; } = "";
    public string FormattedConsumerOrganisationType { get; set; } = "";
    public string ProviderPublisher { get; set; } = "";

    public ProviderError ProviderError { get; set; }

    public bool DisplayProvider => ProviderOdsCodeFound;
    public bool DisplayConsumer => ConsumerOdsCodeFound;
    public bool DetailsEnabled => SearchResultsTotalCount > 0 && DisplayProvider && (DisplayConsumer || DisplayConsumerOrganisationType);

    public bool DisplayConsumerOrganisationType => !string.IsNullOrWhiteSpace(FormattedConsumerOrganisationType);

    public int SearchGroupId { get; set; }
    public int SearchResultId { get; set; }

    public bool ProviderEnabledForGpConnectAppointmentManagement { get; set; } = false;
    public bool ConsumerEnabledForGpConnectAppointmentManagement { get; set; } = false;
    public bool ProviderASIDPresent { get; set; } = false;

    public bool CapabilityStatementOk { get; set; } = false;
    public bool SlotSearchOk { get; set; } = false;

    public int ErrorCode { get; set; }

    public string DisplayDetails { get; set; }
}
