﻿using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Helpers.Constants;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Newtonsoft.Json;
using System.Data;
using SearchGroup = GpConnect.AppointmentChecker.Api.DTO.Response.Application.SearchGroup;
using SearchResult = GpConnect.AppointmentChecker.Api.DTO.Response.Application.SearchResult;

namespace GpConnect.AppointmentChecker.Api.Service;

public class ApplicationService : IApplicationService
{
    private readonly IDataService _dataService;
    private readonly ILogService _logService;

    public ApplicationService(IDataService dataService, ILogService logService)
    {
        _dataService = dataService;
        _logService = logService;
    }
    
    public async Task SynchroniseOrganisation(DTO.Response.Spine.Organisation organisation)
    {
        if (organisation != null)
        {
            var functionName = "application.synchronise_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", organisation.OdsCode);
            parameters.Add("_organisation_type_name", organisation.OrganisationTypeCode);
            parameters.Add("_organisation_name", organisation.OrganisationName);
            parameters.Add("_address_line_1", organisation.PostalAddressFields[0]);
            parameters.Add("_address_line_2", organisation.PostalAddressFields[1]);
            parameters.Add("_locality", organisation.PostalAddressFields[2]);
            parameters.Add("_city", organisation.PostalAddressFields[3]);
            parameters.Add("_county",
                organisation.PostalAddressFields.Length > 4 ? organisation.PostalAddressFields[4] : string.Empty);
            parameters.Add("_postcode", organisation.PostalCode);
            await _dataService.ExecuteQuery(functionName, parameters);
        }
    }

    public async Task<SearchGroup> AddSearchGroup(DTO.Request.Application.SearchGroup searchGroup)
    {
        var functionName = "application.add_search_group";
        var parameters = new DynamicParameters();
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId));
        parameters.Add("_consumer_ods_textbox", searchGroup.ConsumerOdsTextbox);
        parameters.Add("_provider_ods_textbox", searchGroup.ProviderOdsTextbox);
        parameters.Add("_search_date_range", searchGroup.SearchDateRange);
        parameters.Add("_search_start_at", DateTime.UtcNow);
        parameters.Add("_consumer_organisation_type_dropdown", searchGroup.ConsumerOrganisationTypeDropdown);
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchGroup>(functionName, parameters);
        return result;
    }

    public async Task UpdateSearchGroup(int searchGroupId)
    {
        var functionName = "application.update_search_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId));
        parameters.Add("_search_end_at", DateTime.UtcNow);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<AddSearchResult> AddSearchResult(DTO.Request.Application.SearchResult searchResult)
    {
        var functionName = "application.add_search_result";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchResult.SearchGroupId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<AddSearchResult>(functionName, parameters);

        if (searchResult.SpineMessageId != null && result != null)
        {
            await _logService.UpdateSpineMessageLog(searchResult.SpineMessageId.Value, result.SearchResultId);
        }
        return result;
    }

    public async Task<SearchResult> GetSearchResult(int searchResultId)
    {
        var functionName = "application.get_search_result";
        var parameters = new DynamicParameters();
        parameters.Add("_search_result_id", searchResultId);
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId));
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchResult>(functionName, parameters);
        return result;
    }

    public async Task<SearchGroup> GetSearchGroup(int searchGroupId)
    {
        var functionName = "application.get_search_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId));
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchGroup>(functionName, parameters);
        return result;
    }

    public async Task<List<SearchResponse>> GetSearchResultByGroup(int searchGroupId)
    {
        var functionName = "application.get_search_result_by_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", LoggingHelper.GetIntegerValue(Headers.UserId));
        var searchResultByGroup = await _dataService.ExecuteQuery<SearchResultByGroup>(functionName, parameters);

        var searchResponseList = searchResultByGroup.Select(a =>
        {
            var searchResponseNoResults = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details);
            if (searchResponseNoResults != null)
            {
                return new SearchResponse
                {
                    SearchResultsCurrentCount = searchResponseNoResults.SearchResultsCurrentCount,
                    SearchResultsPastCount = searchResponseNoResults.SearchResultsPastCount,
                    ProviderOdsCodeFound = searchResponseNoResults.ProviderOdsCodeFound,
                    ConsumerOdsCodeFound = searchResponseNoResults.ConsumerOdsCodeFound,
                    ProviderOdsCode = searchResponseNoResults.ProviderOdsCode,
                    ConsumerOdsCode = searchResponseNoResults.ConsumerOdsCode,
                    FormattedConsumerOrganisationType = searchResponseNoResults.FormattedConsumerOrganisationType,
                    FormattedProviderOrganisationDetails = searchResponseNoResults.FormattedProviderOrganisationDetails,
                    FormattedConsumerOrganisationDetails = searchResponseNoResults.FormattedConsumerOrganisationDetails,
                    DisplayDetails = searchResponseNoResults.DisplayDetails,
                    ProviderPublisher = searchResponseNoResults.ProviderPublisher,
                    SearchResultId = searchResponseNoResults.SearchResultId,
                    SearchGroupId = searchResponseNoResults.SearchGroupId,
                    TimeTaken = searchResponseNoResults.TimeTaken,
                    ProviderEnabledForGpConnectAppointmentManagement = searchResponseNoResults.ProviderEnabledForGpConnectAppointmentManagement,
                    ConsumerEnabledForGpConnectAppointmentManagement = searchResponseNoResults.ConsumerEnabledForGpConnectAppointmentManagement,
                    ProviderASIDPresent = searchResponseNoResults.ProviderASIDPresent,
                    CapabilityStatementOk = searchResponseNoResults.CapabilityStatementOk,
                    SlotSearchOk = searchResponseNoResults.SlotSearchOk,
                    ErrorCode = searchResponseNoResults.ErrorCode
                };
            }
            return null;
        }).OrderBy(x => x.SearchResultId).ToList();
        return searchResponseList;
    }

    public async Task UpdateSearchResult(int searchResultId, SearchResponse searchResponse, double timeTaken)
    {
        var searchResponseNoResults = new SearchResponseNoResults()
        {
            SearchResultsCurrentCount = searchResponse.SearchResultsCurrentCount,
            SearchResultsPastCount = searchResponse.SearchResultsPastCount,
            TimeTaken = searchResponse.TimeTaken,
            ProviderOdsCode = searchResponse.ProviderOdsCode,
            ConsumerOdsCode = searchResponse.ConsumerOdsCode,
            ProviderOdsCodeFound = searchResponse.ProviderOdsCodeFound,
            ConsumerOdsCodeFound = searchResponse.ConsumerOdsCodeFound,
            FormattedProviderOrganisationDetails = searchResponse.FormattedProviderOrganisationDetails,
            FormattedConsumerOrganisationDetails = searchResponse.FormattedConsumerOrganisationDetails,
            FormattedConsumerOrganisationType = searchResponse.FormattedConsumerOrganisationType,
            ProviderPublisher = searchResponse.ProviderPublisher,
            ProviderError = searchResponse.ProviderError,
            SearchGroupId = searchResponse.SearchGroupId,
            SearchResultId = searchResponse.SearchResultId,
            ProviderASIDPresent = searchResponse.ProviderASIDPresent,
            ProviderEnabledForGpConnectAppointmentManagement = searchResponse.ProviderEnabledForGpConnectAppointmentManagement,
            ConsumerEnabledForGpConnectAppointmentManagement = searchResponse.ConsumerEnabledForGpConnectAppointmentManagement,
            CapabilityStatementOk = searchResponse.CapabilityStatementOk,
            SlotSearchOk = searchResponse.SlotSearchOk,
            DisplayDetails = searchResponse.DisplayDetails,
            ErrorCode = searchResponse.ErrorCode
        };

        var functionName = "application.update_search_result";
        var parameters = new DynamicParameters();
        parameters.Add("_search_result_id", searchResultId);
        parameters.Add("_details", JsonConvert.SerializeObject(searchResponseNoResults));
        parameters.Add("_error_code", searchResponseNoResults.ErrorCode);
        parameters.Add("_search_duration_seconds", timeTaken, DbType.Double);
        await _dataService.ExecuteQuery(functionName, parameters);
    }
}
