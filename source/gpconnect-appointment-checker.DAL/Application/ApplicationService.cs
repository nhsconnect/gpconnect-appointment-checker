using Dapper;
using gpconnect_appointment_checker.DAL.Interfaces;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.DTO.Response.GpConnect;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Dynamic.Core;

namespace gpconnect_appointment_checker.DAL.Application
{
    public class ApplicationService : IApplicationService
    {
        private readonly ILogger<ApplicationService> _logger;
        private readonly IDataService _dataService;
        private readonly IAuditService _auditService;
        private readonly ILogService _logService;
        private readonly IHttpContextAccessor _context;
        private readonly IEmailService _emailService;

        public ApplicationService(IConfiguration configuration, ILogger<ApplicationService> logger, IDataService dataService, IAuditService auditService, ILogService logService, IHttpContextAccessor context, IEmailService emailService)
        {
            _logger = logger;
            _dataService = dataService;
            _auditService = auditService;
            _logService = logService;
            _context = context;
            _emailService = emailService;
        }

        public Organisation GetOrganisation(string odsCode)
        {
            var functionName = "application.get_organisation";
            var parameters = new DynamicParameters();
            parameters.Add("_ods_code", odsCode, DbType.String, ParameterDirection.Input);
            var result = _dataService.ExecuteFunction<Organisation>(functionName, parameters);
            return result.FirstOrDefault();
        }

        public List<User> GetUsers(SortBy sortByColumn, SortDirection sortDirection, UserAccountStatus? userAccountStatusFilter = null)
        {
            var functionName = "application.get_users";
            var result = _dataService.ExecuteFunction<User>(functionName).AsQueryable();
            if (userAccountStatusFilter != null)
            {
                result = result.Where(x => x.UserAccountStatusId == (int)userAccountStatusFilter.Value);
            }
            result = result.OrderBy($"{sortByColumn} {sortDirection}");
            return result.ToList();
        }

        public List<User> GetAdminUsers()
        {
            var functionName = "application.get_users";
            var result = _dataService.ExecuteFunction<User>(functionName).AsQueryable().Where(x => x.AccessLevel == AccessLevel.Admin.ToString());
            return result.ToList();
        }

        public List<User> FindUsers(string surname, string emailAddress, string organisationName, SortBy sortByColumn)
        {
            var functionName = "application.get_users";
            var result = _dataService.ExecuteFunction<User>(functionName);
            var filteredList = result.AsQueryable();
            filteredList = !string.IsNullOrEmpty(surname) ? filteredList.Where(x => x.DisplayName.Contains(surname, StringComparison.OrdinalIgnoreCase)) : filteredList;
            filteredList = !string.IsNullOrEmpty(emailAddress) ? filteredList.Where(x => x.EmailAddress.Contains(emailAddress, StringComparison.OrdinalIgnoreCase)) : filteredList;
            filteredList = !string.IsNullOrEmpty(organisationName) ? filteredList.Where(x => x.OrganisationName.Contains(organisationName, StringComparison.OrdinalIgnoreCase)) : filteredList;
            return filteredList.OrderBy(sortByColumn.ToString()).ToList();
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
            parameters.Add("_search_duration_seconds", searchResult.SearchDurationSeconds);
            var result = _dataService.ExecuteFunction<SearchResult>(functionName, parameters).FirstOrDefault();

            if (searchResult.SpineMessageId != null && result != null)
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
                DisplayConsumer = a.ConsumerOrganisationName != null,
                DisplayClass = (a.ErrorCode != (int)ErrorCode.None) ? "nhsuk-slot-summary-error" : "nhsuk-slot-summary"
            }).OrderBy(x => x.SearchResultId).ToList();
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

        public User AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount)
        {
            var functionName = "application.add_or_update_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", userCreateAccount.EmailAddress);
            parameters.Add("_display_name", userCreateAccount.DisplayName);
            parameters.Add("_organisation_id", userCreateAccount.OrganisationId);
            parameters.Add("_user_account_status_id", (int)userCreateAccount.UserAccountStatus);
            var user = _dataService.ExecuteFunction<User>(functionName, parameters).FirstOrDefault();
            return user;
        }

        public List<User> SetUserStatus(int[] userId, int[] userAccountStatusId)
        {
            var userStatusUpdateList = new List<User>();
            for (var i = 0; i < userId.Length; i++)
            {
                var functionName = "application.set_user_status";
                var parameters = new DynamicParameters();
                parameters.Add("_admin_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")));
                parameters.Add("_user_id", userId[i]);
                parameters.Add("_user_account_status_id", userAccountStatusId[i]);
                parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")));
                var user = _dataService.ExecuteFunction<User>(functionName, parameters).FirstOrDefault();

                if (user != null && user.StatusChanged)
                {
                    userStatusUpdateList.Add(user);                    
                }
            }
            return userStatusUpdateList;
        }

        public void SetMultiSearch(int userId, bool multiSearchEnabled)
        {
            var functionName = "application.set_multi_search";
            var parameters = new DynamicParameters();
            parameters.Add("_admin_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")));
            parameters.Add("_user_id", userId);
            parameters.Add("_multi_search_enabled", multiSearchEnabled);
            parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")));
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public User AddUser(string emailAddress)
        {
            var functionName = "application.add_user_manual";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", emailAddress);
            parameters.Add("_admin_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")));
            parameters.Add("_user_session_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserSessionId")));
            var user = _dataService.ExecuteFunction<User>(functionName, parameters).FirstOrDefault();
            return user;
        }

        public void UpdateUserTermsAndConditions(bool isAccepted)
        {
            var functionName = "application.update_user_terms_and_conditions";
            var parameters = new DynamicParameters();
            parameters.Add("_user_id", Convert.ToInt32(_context.HttpContext?.User?.GetClaimValue("UserId")));
            parameters.Add("_accepted", isAccepted);
            _dataService.ExecuteFunction(functionName, parameters);
        }

        public User GetUser(string emailAddress)
        {
            var functionName = "application.get_user";
            var parameters = new DynamicParameters();
            parameters.Add("_email_address", emailAddress);
            var user = _dataService.ExecuteFunction<User>(functionName, parameters).FirstOrDefault();
            return user;
        }
    }
}
