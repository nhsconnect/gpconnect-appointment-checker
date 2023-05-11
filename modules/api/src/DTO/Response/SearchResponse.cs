using GpConnect.AppointmentChecker.Api.DTO.Response.GpConnect;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;

namespace GpConnect.AppointmentChecker.Api.DTO.Response;

public class SearchResponse
{
    public int? SearchResultsTotalCount => SearchResultsCurrentCount + SearchResultsPastCount;
    public int? SearchResultsCurrentCount => SearchResults?.Count;
    public int? SearchResultsPastCount => SearchResultsPast?.Count;

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

    public bool DisplayProvider => ProviderError == null && ProviderOdsCodeFound;
    public bool DisplayConsumer => ConsumerOdsCodeFound;
    public bool DetailsEnabled => SearchResults?.Count > 0 && DisplayProvider && DisplayConsumer;

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
