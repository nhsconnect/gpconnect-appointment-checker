using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Request.Application;
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
    private readonly IApplicationService _applicationService;

    public SearchService(ITokenService tokenService, ISpineService spineService, ICapabilityStatement capabilityStatement, IGpConnectQueryExecutionService gpConnectQueryExecutionService, IApplicationService applicationService)
    {
        _tokenService = tokenService;
        _spineService = spineService;
        _capabilityStatement = capabilityStatement;
        _gpConnectQueryExecutionService = gpConnectQueryExecutionService;
        _applicationService = applicationService;
    }

    public async Task<List<SearchResponse>> ExecuteSearch(SearchRequest searchRequest)
    {
        var createdSearchGroup = await AddSearchGroupToSearchResponse(searchRequest);        

        var searchResponses = new List<SearchResponse>();
        for (var providerCodeIndex = 0; providerCodeIndex < searchRequest.ProviderOdsCodeAsList.Count; providerCodeIndex++)
        {
            for (var consumerCodeIndex = 0; consumerCodeIndex < searchRequest.ConsumerOdsCodeAsList.Count; consumerCodeIndex++)
            {
                searchResponses.Add(await ProcessSearchRequestInstance(createdSearchGroup.SearchGroupId, searchRequest, providerCodeIndex, consumerCodeIndex));
            }
        }
        return searchResponses;
    }

    private async Task<SearchResponse> ProcessSearchRequestInstance(int searchGroupId, SearchRequest searchRequest, int providerCodeIndex, int consumerCodeIndex)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var providerOdsCode = searchRequest.ProviderOdsCodeAsList[providerCodeIndex].ToUpper();
        var consumerOdsCode = searchRequest.ConsumerOdsCodeAsList[consumerCodeIndex].ToUpper();

        var searchResponse = new SearchResponse() 
        { 
            ProviderOdsCode = providerOdsCode, 
            ConsumerOdsCode = consumerOdsCode
        };       

        var providerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(providerOdsCode);
        var consumerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(consumerOdsCode);

        searchResponse.ProviderOdsCodeFound = providerOrganisationDetails != null;
        searchResponse.ConsumerOdsCodeFound = consumerOrganisationDetails != null;

        searchResponse.FormattedProviderOrganisationDetails = providerOrganisationDetails?.OrganisationLocationWithOdsCode;
        searchResponse.FormattedConsumerOrganisationDetails = consumerOrganisationDetails?.OrganisationLocationWithOdsCode;
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
                    ProviderOrganisationDetails = new DTO.Request.GpConnect.OrganisationRequestParameters() { OdsCode = providerOrganisationDetails?.OdsCode },
                    ConsumerOrganisationDetails = new DTO.Request.GpConnect.OrganisationRequestParameters() { OdsCode = consumerOrganisationDetails?.OdsCode },
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
        stopwatch.Stop();
        searchResponse.TimeTaken = stopwatch.Elapsed;
        searchResponse.SearchGroupId = searchGroupId;

        var createdSearchResult = await AddSearchResultToSearchGroup(providerOdsCode, consumerOdsCode, searchRequest.ConsumerOrganisationType, searchResponse);
        searchResponse.SearchResultId = createdSearchResult.SearchResultId;

        return searchResponse;
    }

    private async Task<DTO.Response.Application.AddSearchResult> AddSearchResultToSearchGroup(string providerOdsCode, string consumerOdsCode, string consumerOrganisationType, SearchResponse searchResponse)
    {
        var createdSearchResult = await _applicationService.AddSearchResult(new SearchResult
        {
            SearchGroupId = searchResponse.SearchGroupId,
            ProviderCode = providerOdsCode,
            ConsumerCode = consumerOdsCode,
            ConsumerOrganisationType = consumerOrganisationType,
            ProviderPublisher = searchResponse.ProviderPublisher            
        });
        return createdSearchResult;
    }

    private async Task<DTO.Response.Application.SearchGroup> AddSearchGroupToSearchResponse(SearchRequest searchRequest)
    {
        var createdSearchGroup = await _applicationService.AddSearchGroup(new SearchGroup
        {
            UserSessionId = searchRequest.UserSessionId,
            ProviderOdsTextbox = searchRequest.ProviderOdsCode,
            ConsumerOdsTextbox = searchRequest.ConsumerOdsCode,
            SearchDateRange = searchRequest.DateRange,
            ConsumerOrganisationTypeDropdown = searchRequest.ConsumerOrganisationType
        });
        return createdSearchGroup;
    }
}
