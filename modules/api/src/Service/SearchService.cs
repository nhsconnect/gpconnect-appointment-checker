using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using System.Diagnostics;

namespace GpConnect.AppointmentChecker.Api.Service;

public class SearchService : ISearchService
{
    private readonly ITokenService _tokenService;
    private readonly ISpineService _spineService;
    private readonly ICapabilityStatement _capabilityStatement;
    private readonly IGpConnectQueryExecutionService _gpConnectQueryExecutionService;

    public SearchService(ITokenService tokenService, ISpineService spineService, ICapabilityStatement capabilityStatement, IGpConnectQueryExecutionService gpConnectQueryExecutionService)
    {
        _tokenService = tokenService;
        _spineService = spineService;
        _capabilityStatement = capabilityStatement;
        _gpConnectQueryExecutionService = gpConnectQueryExecutionService;
    }

    public async Task<List<SearchResponse>> ExecuteSearch(SearchRequest searchRequest)
    {
        var searchResponses = new List<SearchResponse>();
        for (var providerCodeIndex = 0; providerCodeIndex < searchRequest.ProviderOdsCodeAsList.Count; providerCodeIndex++)
        {
            for (var consumerCodeIndex = 0; consumerCodeIndex < searchRequest.ConsumerOdsCodeAsList.Count; consumerCodeIndex++)
            {
                searchResponses.Add(await ProcessSearchRequestInstance(searchRequest, providerCodeIndex, consumerCodeIndex));
            }
        }
        return searchResponses;
    }

    private async Task<SearchResponse> ProcessSearchRequestInstance(SearchRequest searchRequest, int providerCodeIndex, int consumerCodeIndex)
    {
        var stopwatch = new Stopwatch();
        var searchResponse = new SearchResponse();
        stopwatch.Start();

        var providerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(searchRequest.ProviderOdsCodeAsList[providerCodeIndex].ToUpper());
        var consumerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(searchRequest.ConsumerOdsCodeAsList[consumerCodeIndex].ToUpper());

        searchResponse.ProviderOdsCodeFound = providerOrganisationDetails != null;
        searchResponse.ConsumerOdsCodeFound = consumerOrganisationDetails != null;

        if (providerOrganisationDetails != null && (consumerOrganisationDetails != null || !string.IsNullOrWhiteSpace(searchRequest.ConsumerOrganisationType)))
        {
            searchResponse.FormattedProviderOrganisationDetails = providerOrganisationDetails.FormattedOrganisationDetails;
            searchResponse.FormattedConsumerOrganisationDetails = consumerOrganisationDetails?.FormattedOrganisationDetails;
            searchResponse.FormattedConsumerOrganisationType = searchRequest.ConsumerOrganisationType;

            var providerSpineDetails = await _spineService.GetProviderDetails(searchRequest.ProviderOdsCodeAsList[providerCodeIndex].ToUpper());
            var consumerSpineDetails = await _spineService.GetConsumerDetails(searchRequest.ConsumerOdsCodeAsList[consumerCodeIndex].ToUpper());

            searchResponse.ProviderEnabledForGpConnectAppointmentManagement = providerSpineDetails != null;
            searchResponse.ConsumerEnabledForGpConnectAppointmentManagement = (consumerSpineDetails != null && consumerSpineDetails.HasAsId) || !string.IsNullOrWhiteSpace(searchRequest.ConsumerOrganisationType);

            if (providerSpineDetails != null)
            {
                searchResponse.ProviderASIDPresent = providerSpineDetails.HasAsId;
                searchResponse.ProviderPublisher = providerSpineDetails.ProductName;

                if (searchResponse.ProviderASIDPresent)
                {
                    var requestParameters = await _tokenService.ConstructRequestParameters(new DTO.Request.GpConnect.RequestParameters()
                    {
                        RequestUri = searchRequest.RequestUri,
                        ProviderSpineDetails = new DTO.Request.GpConnect.SpineProviderRequestParameters() { EndpointAddress = providerSpineDetails.EndpointAddress, AsId = providerSpineDetails.AsId },
                        ConsumerOrganisationType = searchRequest.ConsumerOrganisationType,
                        ProviderOrganisationDetails = new DTO.Request.GpConnect.OrganisationRequestParameters() { OdsCode = providerOrganisationDetails.OdsCode },
                        ConsumerOrganisationDetails = new DTO.Request.GpConnect.OrganisationRequestParameters() { OdsCode = consumerOrganisationDetails.OdsCode },
                        SpineMessageTypeId = SpineMessageTypes.GpConnectSearchFreeSlots,
                        UserId = searchRequest.UserId,
                        Sid = searchRequest.Sid
                    });

                    if (requestParameters != null)
                    {
                        var capabilityStatement = await _capabilityStatement.GetCapabilityStatement(requestParameters, providerSpineDetails.SspHostname);

                        if (capabilityStatement.NoIssues)
                        {
                            searchResponse.CapabilityStatementOk = true;
                            var searchResults = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearch(requestParameters, searchRequest.StartDate, searchRequest.EndDate, providerSpineDetails.SspHostname, searchRequest.UserId);
                            if (searchResults.NoIssues)
                            {
                                searchResponse.SearchResults = searchResults.CurrentSlotEntrySimple;
                                searchResponse.SearchResultsPast = searchResults.PastSlotEntrySimple;
                                searchResponse.SlotSearchOk = true;
                            }
                            else if (!searchResults.NoIssues)
                            {
                                searchResponse.ProviderError = new ProviderError()
                                {
                                    Display = searchResults.ProviderError,
                                    Code = searchResults.ProviderErrorCode,
                                    Diagnostics = searchResults.ProviderErrorDiagnostics
                                };
                            }
                        }
                        else if (!capabilityStatement.NoIssues)
                        {
                            searchResponse.ProviderError = new ProviderError()
                            {
                                Display = capabilityStatement.ProviderError,
                                Code = capabilityStatement.ProviderErrorCode,
                                Diagnostics = capabilityStatement.ProviderErrorDiagnostics
                            };
                        }
                    }
                }
            }
        }
        stopwatch.Stop();
        searchResponse.TimeTaken = stopwatch.Elapsed;
        return searchResponse;
    }
}
