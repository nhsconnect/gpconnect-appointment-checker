using System;
using System.Collections.Generic;
using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Linq;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DAL.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IDataService _dataService;
        private readonly IAuditService _auditService;
        private readonly ILogService _logService;

        public ApplicationService(IConfiguration configuration, ILogger<ApplicationService> logger, IDataService dataService, IAuditService auditService, ILogService logService)
        {
            _logger = logger;
            _dataService = dataService;
            _auditService = auditService;
            _logService = logService;
        }

        public Organisation GetOrganisation(string odsCode)
        {
            var functionName = "application.get_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<Organisation>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public void SynchroniseOrganisation(Organisation organisation)
        {
            if (organisation != null)
            {
                var functionName = "application.synchronise_organisation";
                var parameters = new DynamicParameters();
                parameters.Add("_ods_code", organisation.ODSCode);
                parameters.Add("_organisation_type_name", organisation.OrganisationTypeCode);
                parameters.Add("_organisation_name", organisation.OrganisationName);
                parameters.Add("_address_line_1", organisation.PostalAddressFields[0]);
                parameters.Add("_address_line_2", organisation.PostalAddressFields[1]);
                parameters.Add("_locality", organisation.PostalAddressFields[2]);
                parameters.Add("_city", organisation.PostalAddressFields[3]);
                parameters.Add("_county",
                    organisation.PostalAddressFields.Length > 4 ? organisation.PostalAddressFields[4] : string.Empty);
                parameters.Add("_postcode", organisation.PostalCode);
                _dataService.ExecuteFunction(functionName, parameters);
            }
        }

        public SearchGroup AddSearchGroup(DTO.Request.Application.SearchGroup searchGroup)
        {
            var functionName = "application.add_search_group";
            var parameters = new DynamicParameters();
            parameters.Add("_user_session_id", searchGroup.UserSessionId);
            parameters.Add("_consumer_ods_textbox", searchGroup.ConsumerOdsTextbox);
            parameters.Add("_provider_ods_textbox", searchGroup.ProviderOdsTextbox);
            parameters.Add("_search_date_range", searchGroup.SearchDateRange);
            parameters.Add("_search_start_at", searchGroup.SearchStartAt);
            parameters.Add("_search_end_at", DBNull.Value, DbType.DateTime);
            var result = _dataService.ExecuteFunction<SearchGroup>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public SearchResult AddSearchResult(DTO.Request.Application.SearchResult searchResult)
        {
            var functionName = "application.add_search_result";
            var parameters = new DynamicParameters();
            parameters.Add("_search_group_id", searchResult.SearchGroupId);
            parameters.Add("_provider_ods_code", searchResult.ProviderCode);
            parameters.Add("_consumer_ods_code", searchResult.ConsumerCode);
            parameters.Add("_error_code", searchResult.ErrorCode);
            parameters.Add("_details", searchResult.Details);
            parameters.Add("_provider_publisher", searchResult.ProviderPublisher);
            var result = _dataService.ExecuteFunction<SearchResult>(functionName, parameters).FirstOrDefault();

            if(searchResult.SpineMessageId != null)
            {
                _logService.UpdateSpineMessageLog(searchResult.SpineMessageId.Value, result.SearchResultId);
            }
            return result;
        }

        public SearchGroup GetSearchGroup(int searchGroupId, int userId)
        {
            var functionName = "application.get_search_group";
            var parameters = new DynamicParameters();
            parameters.Add("_search_group_id", searchGroupId);
            parameters.Add("_user_id", userId);
            var result = _dataService.ExecuteFunction<SearchGroup>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public SearchResult GetSearchResult(int searchResultId, int userId)
        {
            var functionName = "application.get_search_result";
            var parameters = new DynamicParameters();
            parameters.Add("_search_result_id", searchResultId);
            parameters.Add("_user_id", userId);
            var result = _dataService.ExecuteFunction<SearchResult>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public List<SlotEntrySummary> GetSearchResultByGroup(int searchGroupId, int userId)
        {
            var functionName = "application.get_search_result_by_group";
            var parameters = new DynamicParameters();
            parameters.Add("_search_group_id", searchGroupId);
            parameters.Add("_user_id", userId);
            var searchResultByGroup = _dataService.ExecuteFunction<SearchResultByGroup>(functionName, parameters);
            var slotEntrySummaryList = searchResultByGroup.Select(a => new SlotEntrySummary
            {
                ProviderLocationName = $"{a.ProviderOrganisationName}, {StringExtensions.AddressBuilder(new List<string>(a.ProviderAddressFields), a.ProviderPostcode)}",
                ProviderOdsCode = a.ProviderOdsCode,
                ConsumerLocationName = $"{a.ConsumerOrganisationName}, {StringExtensions.AddressBuilder(new List<string>(a.ConsumerAddressFields), a.ConsumerPostcode)}",
                ConsumerOdsCode = a.ConsumerOdsCode,
                SearchSummaryDetail = a.Details,
                ProviderPublisher = a.ProviderPublisher,
                SearchResultId = a.SearchResultId,
                DetailsEnabled = a.ErrorCode == (int)ErrorCode.None,
                DisplayProvider = a.ProviderOrganisationName != null,
                DisplayConsumer = a.ConsumerOrganisationName != null
            }).ToList();
            return slotEntrySummaryList;
        }

        public User LogonUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logon_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_display_name", user.DisplayName);
            parameters.Add("_organisation_id", user.OrganisationId);
            var result = _dataService.ExecuteFunction<User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public User LogoffUser(DTO.Request.Application.User user)
        {
            var functionName = "application.logoff_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_user_session_id", user.UserSessionId);
            var result = _dataService.ExecuteFunction<User>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public void SetUserAuthorised(DTO.Request.Application.User user)
        {
            var functionName = "application.set_user_isauthorised";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", user.EmailAddress);
            parameters.Add("_is_authorised", user.IsAuthorised);
            _dataService.ExecuteFunction(functionName, parameters);
        }
    }
}
