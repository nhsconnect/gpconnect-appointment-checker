using Dapper;
using GpConnect.AppointmentChecker.Api.DAL.Interfaces;
using GpConnect.AppointmentChecker.Api.DTO.Response;
using GpConnect.AppointmentChecker.Api.DTO.Response.Application;
using GpConnect.AppointmentChecker.Api.Helpers;
using GpConnect.AppointmentChecker.Api.Service.Interfaces;
using Newtonsoft.Json;
using NLog.LayoutRenderers;
using Notify.Models.Responses;
using System.Data;
using SearchExport = GpConnect.AppointmentChecker.Api.DTO.Response.Application.SearchExport;
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

    public async Task<Organisation> GetOrganisation(string odsCode)
    {
        var functionName = "application.get_organisation";
        var parameters = new DynamicParameters();
        parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
        var result = await _dataService.ExecuteQueryFirstOrDefault<Organisation>(functionName, parameters);
        return result;
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
        parameters.Add("_user_session_id", searchGroup.UserSessionId);
        parameters.Add("_consumer_ods_textbox", searchGroup.ConsumerOdsTextbox);
        parameters.Add("_provider_ods_textbox", searchGroup.ProviderOdsTextbox);
        parameters.Add("_search_date_range", searchGroup.SearchDateRange);
        parameters.Add("_search_start_at", DateTime.UtcNow);
        parameters.Add("_consumer_organisation_type_dropdown", searchGroup.ConsumerOrganisationTypeDropdown);
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchGroup>(functionName, parameters);
        return result;
    }

    public async Task UpdateSearchGroup(int searchGroupId, int userId)
    {
        var functionName = "application.update_search_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", userId);
        parameters.Add("_search_end_at", DateTime.UtcNow);
        await _dataService.ExecuteQuery(functionName, parameters);
    }

    public async Task<AddSearchResult> AddSearchResult(DTO.Request.Application.SearchResult searchResult)
    {
        var functionName = "application.add_search_result";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchResult.SearchGroupId);
        //parameters.Add("_provider_ods_code", searchResult.ProviderCode);
        //parameters.Add("_consumer_ods_code", searchResult.ConsumerCode);
        //parameters.Add("_error_code", searchResult.ErrorCode);
        //parameters.Add("_details", searchResult.Details);
        //parameters.Add("_provider_publisher", searchResult.ProviderPublisher);
        //parameters.Add("_search_duration_seconds", searchResult.SearchDurationSeconds);
        //parameters.Add("_consumer_organisation_type", searchResult.ConsumerOrganisationType);
        var result = await _dataService.ExecuteQueryFirstOrDefault<AddSearchResult>(functionName, parameters);

        if (searchResult.SpineMessageId != null && result != null)
        {
            await _logService.UpdateSpineMessageLog(searchResult.SpineMessageId.Value, result.SearchResultId);
        }
        return result;
    }

    public async Task<AddSearchExport> AddSearchExport(DTO.Request.Application.SearchExport searchExport)
    {
        var functionName = "application.add_search_export";
        var parameters = new DynamicParameters();
        parameters.Add("_search_export_data", searchExport.SearchExportData);
        parameters.Add("_user_id", searchExport.UserId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<AddSearchExport>(functionName, parameters);
        return result;
    }

    public async Task<SearchExport> GetSearchExport(int searchExportId, int userId)
    {
        var functionName = "application.get_search_export";
        var parameters = new DynamicParameters();
        parameters.Add("_search_export_id", searchExportId);
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchExport>(functionName, parameters);
        //return result.SearchExportData.ConvertJsonDataToDataTable();
        return result;
    }

    public async Task<List<SearchGroupExport>> GetSearchGroupExport(int searchGroupId, int userId)
    {
        var functionName = "application.get_search_result_by_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQuery<SearchGroupExport>(functionName, parameters);
        return result;
        //var json = result.ConvertObjectToJsonData();
        //return json.ConvertJsonDataToDataTable();
    }

    public async Task<SearchGroup> GetSearchGroup(int searchGroupId, int userId)
    {
        var functionName = "application.get_search_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchGroup>(functionName, parameters);
        return result;
    }

    public async Task<SearchResult> GetSearchResult(int searchResultId, int userId)
    {
        var functionName = "application.get_search_result";
        var parameters = new DynamicParameters();
        parameters.Add("_search_result_id", searchResultId);
        parameters.Add("_user_id", userId);
        var result = await _dataService.ExecuteQueryFirstOrDefault<SearchResult>(functionName, parameters);
        return result;
    }

    public async Task<List<SearchResponse>> GetSearchResultByGroup(int searchGroupId, int userId)
    {
        var functionName = "application.get_search_result_by_group";
        var parameters = new DynamicParameters();
        parameters.Add("_search_group_id", searchGroupId);
        parameters.Add("_user_id", userId);
        var searchResultByGroup = await _dataService.ExecuteQuery<SearchResultByGroup>(functionName, parameters);

        var searchResponseList = searchResultByGroup.Select(a => new SearchResponse
        {   
            SearchResultsCurrentCount = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).SearchResultsCurrentCount,
            SearchResultsPastCount = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).SearchResultsPastCount,
            ProviderOdsCodeFound = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ProviderOdsCodeFound,
            ConsumerOdsCodeFound = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ConsumerOdsCodeFound,
            ProviderOdsCode = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ProviderOdsCode,
            ConsumerOdsCode = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ConsumerOdsCode,
            FormattedConsumerOrganisationType = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).FormattedConsumerOrganisationType,
            FormattedProviderOrganisationDetails = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).FormattedProviderOrganisationDetails,
            FormattedConsumerOrganisationDetails = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).FormattedConsumerOrganisationDetails,
            DisplayDetails = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).DisplayDetails,
            ProviderPublisher = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ProviderPublisher,
            SearchResultId = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).SearchResultId,
            SearchGroupId = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).SearchGroupId,
            TimeTaken = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).TimeTaken,
            ProviderEnabledForGpConnectAppointmentManagement = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ProviderEnabledForGpConnectAppointmentManagement,
            ConsumerEnabledForGpConnectAppointmentManagement = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ConsumerEnabledForGpConnectAppointmentManagement,
            ProviderASIDPresent = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ProviderASIDPresent,
            CapabilityStatementOk = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).CapabilityStatementOk,
            SlotSearchOk = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).SlotSearchOk,
            ErrorCode = JsonConvert.DeserializeObject<SearchResponseNoResults>(a.Details).ErrorCode,

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
        parameters.Add("_error_code", 0);
        parameters.Add("_search_duration_seconds", timeTaken, DbType.Double);
        await _dataService.ExecuteQuery(functionName, parameters);
    }
}
