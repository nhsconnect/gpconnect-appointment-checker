﻿using GpConnect.AppointmentChecker.Api.DTO.Request;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using GpConnect.AppointmentChecker.Api.Service.Interfaces.GpConnect;
using System.Diagnostics;
using SearchGroup = GpConnect.AppointmentChecker.Api.DTO.Request.Application.SearchGroup;
using SearchResult = GpConnect.AppointmentChecker.Api.DTO.Request.Application.SearchResult;

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

    public async Task<SearchResponse> ExecuteFreeSlotSearchFromDatabase(SearchFromDatabaseRequest searchFromDatabaseRequest)
    {
        var response = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearchFromDatabase(searchFromDatabaseRequest.SearchResultId, searchFromDatabaseRequest.UserId);
        if (response != null)
        {
            var searchResult = await _applicationService.GetSearchResult(searchFromDatabaseRequest.SearchResultId, searchFromDatabaseRequest.UserId);
            var providerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(searchResult.ProviderOdsCode);
            var consumerOrganisationDetails = await _spineService.GetOrganisationDetailsByOdsCodeAsync(searchResult.ConsumerOdsCode);

            var searchResponse = new SearchResponse()
            {
                SearchResults = response.CurrentSlotEntrySimple,
                SearchResultsPast = response.PastSlotEntrySimple,
                ProviderOdsCode = searchResult.ProviderOdsCode,
                ConsumerOdsCode = searchResult.ConsumerOdsCode,
                TimeTaken = searchResult.SearchDurationSeconds,
                ProviderOdsCodeFound = true,
                ConsumerOdsCodeFound = true,
                SearchGroupId = searchResult.SearchGroupId,
                SearchResultId = searchResult.SearchResultId,
                FormattedProviderOrganisationDetails = providerOrganisationDetails?.OrganisationLocationWithOdsCode,
                FormattedConsumerOrganisationDetails = consumerOrganisationDetails?.OrganisationLocationWithOdsCode,
                FormattedConsumerOrganisationType = searchResult.ConsumerOrganisationType
            };

            return searchResponse;
        }
        return null;
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
            ConsumerOdsCode = consumerOdsCode,
            SearchGroupId = searchGroupId
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

                        var createdSearchResult = await AddSearchResultToSearchGroup(providerOdsCode, consumerOdsCode, searchRequest.ConsumerOrganisationType, searchResponse.SearchGroupId, searchResponse.ProviderPublisher);
                        searchResponse.SearchResultId = createdSearchResult.SearchResultId;

                        var searchResults = await _gpConnectQueryExecutionService.ExecuteFreeSlotSearch(requestParameters, searchRequest.StartDate, searchRequest.EndDate, providerSpineDetails.SspHostname, searchRequest.UserId, searchResponse.SearchResultId);
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
        
        var details = GetDetails(providerOdsCode, consumerOdsCode, searchResponse.ProviderEnabledForGpConnectAppointmentManagement, searchResponse.ConsumerEnabledForGpConnectAppointmentManagement, searchResponse.ProviderOdsCodeFound, searchResponse.ConsumerOdsCodeFound, searchResponse.SearchResultsCurrentCount, searchResponse.SearchResultsPastCount, searchResponse.SearchResultsTotalCount, searchResponse.ProviderError, searchResponse.ProviderASIDPresent);

        searchResponse.DisplayDetails = details.displayDetail;
        searchResponse.ErrorCode = details.errorCode;

        stopwatch.Stop();
        searchResponse.TimeTaken = stopwatch.Elapsed.TotalSeconds;

        await _applicationService.UpdateSearchGroup(searchGroupId, searchRequest.UserId);
        await _applicationService.UpdateSearchResult(searchResponse.SearchResultId, searchResponse.DisplayDetails, searchResponse.ErrorCode, searchResponse.TimeTaken);

        return searchResponse;
    }

    struct Details
    {
        public string displayDetail;
        public int errorCode;
    }

    private Details GetDetails(string providerOdsCode, string consumerOdsCode, bool providerEnabledForGpConnectAppointmentManagement, bool consumerEnabledForGpConnectAppointmentManagement, bool providerOdsCodeFound, bool consumerOdsCodeFound, int? searchResultsCurrentCount, int? searchResultsPastCount, int? searchResultsTotalCount, ProviderError providerError, bool providerAsidFound)
    {
        Details details;
        details.errorCode = 1;

        var detailsBuilder = new List<string>();

        if (!providerOdsCodeFound)
            detailsBuilder.Add(string.Format(SearchConstants.ISSUEWITHPROVIDERODSCODETEXT, providerOdsCode));
        if (providerOdsCodeFound && !providerEnabledForGpConnectAppointmentManagement)
            detailsBuilder.Add(string.Format(SearchConstants.ISSUEWITHGPCONNECTPROVIDERNOTENABLEDTEXT, providerOdsCode));

        if (!consumerOdsCodeFound)
            detailsBuilder.Add(string.Format(SearchConstants.ISSUEWITHCONSUMERODSCODETEXT, consumerOdsCode));
        if (consumerOdsCodeFound && !consumerEnabledForGpConnectAppointmentManagement)
            detailsBuilder.Add(string.Format(SearchConstants.ISSUEWITHGPCONNECTCONSUMERNOTENABLEDTEXT, consumerOdsCode));

        if (providerOdsCodeFound && !providerAsidFound)
            detailsBuilder.Add(SearchConstants.ISSUEWITHGPCONNECTPROVIDERTEXT);

        if (providerError != null)
            detailsBuilder.Add(string.Format(SearchConstants.ISSUEWITHSENDINGMESSAGETOPROVIDERSYSTEMTEXT, providerError.Display, providerError.Code));

        if (providerOdsCodeFound && consumerOdsCodeFound)
        {
            if (searchResultsTotalCount == 0)
            {
                detailsBuilder.Add(SearchConstants.SEARCHRESULTSNOAVAILABLEAPPOINTMENTSLOTSTEXT);
            }
            else
            {
                if (searchResultsPastCount > 0)
                    detailsBuilder.Add(StringExtensions.Pluraliser(SearchConstants.SEARCHSTATSPASTCOUNTTEXT, searchResultsPastCount.Value));
                if (searchResultsCurrentCount > 0)
                    detailsBuilder.Add(StringExtensions.Pluraliser(SearchConstants.SEARCHSTATSCOUNTTEXT, searchResultsCurrentCount.Value));
            }
            details.errorCode = 0;
        }

        details.displayDetail = detailsBuilder.ConvertObjectToJsonData();
        return details;
    }

    private async Task<DTO.Response.Application.AddSearchResult> AddSearchResultToSearchGroup(string providerOdsCode, string consumerOdsCode, string consumerOrganisationType, int searchGroupId, string providerPublisher)
    {
        var createdSearchResult = await _applicationService.AddSearchResult(new SearchResult
        {
            SearchGroupId = searchGroupId,
            ProviderCode = providerOdsCode,
            ConsumerCode = consumerOdsCode,
            ConsumerOrganisationType = consumerOrganisationType,
            ProviderPublisher = providerPublisher
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
