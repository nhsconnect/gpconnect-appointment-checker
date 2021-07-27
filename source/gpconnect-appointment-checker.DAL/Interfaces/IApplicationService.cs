using System.Collections.Generic;
using gpconnect_appointment_checker.DTO.Response.Application;
using gpconnect_appointment_checker.Helpers.Enumerations;

namespace gpconnect_appointment_checker.DAL.Interfaces
{
    public interface IApplicationService
    {
        DTO.Response.Application.Organisation GetOrganisation(string odsCode);
        void SynchroniseOrganisation(DTO.Response.Application.Organisation organisation);
        DTO.Response.Application.User GetUser(string emailAddress);
        DTO.Response.Application.User LogonUser(DTO.Request.Application.User user);
        DTO.Response.Application.User LogoffUser(DTO.Request.Application.User user);
        DTO.Response.Application.SearchGroup AddSearchGroup(DTO.Request.Application.SearchGroup searchGroup);
        DTO.Response.Application.SearchResult AddSearchResult(DTO.Request.Application.SearchResult searchResult);
        DTO.Response.Application.SearchGroup GetSearchGroup(int searchGroupId, int userId);
        DTO.Response.Application.SearchResult GetSearchResult(int searchResultId, int userId);
        List<DTO.Response.GpConnect.SlotEntrySummary> GetSearchResultByGroup(int searchGroupId, int userId);
        List<DTO.Response.Application.User> GetUsers(SortBy sortBy, SortDirection sortDirection, UserAccountStatus? userAccountStatusFilter = null);
        List<DTO.Response.Application.User> GetAdminUsers();
        List<DTO.Response.Application.User> FindUsers(string surname, string emailAddress, string organisationName, SortBy sortBy);
        User SetUserStatus(int userId, int userAccountStatusId);
        void SetMultiSearch(int userId, bool multiSearchEnabled);
        User AddUser(string emailAddress);
        void UpdateUserTermsAndConditions(bool isAccepted);
        User AddOrUpdateUser(DTO.Request.Application.UserCreateAccount userCreateAccount);
    }
}
