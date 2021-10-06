using gpconnect_appointment_checker.DTO.Response.Application;
using System.Collections.Generic;
using System.Data;

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
        DataTable GetSearchGroupExport(int searchGroupId, int userId);
        DataTable GetSearchExport(int searchExportId, int userId);
        SearchGroup GetSearchGroup(int searchGroupId, int userId);
        void UpdateSearchGroup(int searchGroupId);
        SearchResult GetSearchResult(int searchResultId, int userId);
        List<DTO.Response.GpConnect.SlotEntrySummary> GetSearchResultByGroup(int searchGroupId, int userId);
        List<User> GetUsers(string surname, string emailAddress, string organisationName, string sortBy, string sortDirection, string userAccountStatusFilter, string accessLevelFilter, string multiSearchFilter, string orgTypeSearchFilter);
        List<User> GetUsers(string sortBy, string sortDirection);
        User SetUserStatus(int userId, int userAccountStatusId);
        void SetMultiSearch(int userId, bool multiSearchEnabled);
        void SetOrgTypeSearch(int userId, bool orgTypeSearchEnabled);
        User AddUser(string emailAddress);
        void UpdateUserTermsAndConditions(bool isAccepted);
        User AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
