using System.Collections.Generic;
using System.Data;
using System.IO;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IApplicationService
    {
        Organisation GetOrganisation(string odsCode);
        void SynchroniseOrganisation(Organisation organisation);
        User GetUser(string emailAddress);
        User LogonUser(DTO.Request.Application.User user);
        User LogoffUser(DTO.Request.Application.User user);
        SearchGroup AddSearchGroup(DTO.Request.Application.SearchGroup searchGroup);
        SearchResult AddSearchResult(DTO.Request.Application.SearchResult searchResult);
        SearchExport AddSearchExport(DTO.Request.Application.SearchExport searchExport);
        DataTable GetSearchExport(int searchExportId, int userId);
        SearchGroup GetSearchGroup(int searchGroupId, int userId);
        SearchResult GetSearchResult(int searchResultId, int userId);
        List<DTO.Response.GpConnect.SlotEntrySummary> GetSearchResultByGroup(int searchGroupId, int userId);
        List<User> GetUsers(string surname, string emailAddress, string organisationName, SortBy sortBy, SortDirection sortDirection, UserAccountStatus? userAccountStatusFilter = null, AccessLevel? accessLevelFilter = null, bool? multiSearchFilter = null, bool? orgTypeSearchFilter = null);
        List<User> GetUsers(SortBy sortBy, SortDirection sortDirection);
        User SetUserStatus(int userId, int userAccountStatusId);
        void SetMultiSearch(int userId, bool multiSearchEnabled);
        void SetOrgTypeSearch(int userId, bool orgTypeSearchEnabled);
        User AddUser(string emailAddress);
        void UpdateUserTermsAndConditions(bool isAccepted);
        User AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
