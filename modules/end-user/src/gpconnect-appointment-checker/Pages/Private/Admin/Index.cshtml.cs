using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;

namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel : BaseModel
    {
        private readonly IUserService _userService;

        public AdminModel(IUserService userService, IOptions<GeneralConfig> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {
            _userService = userService;
        }

        public async Task OnGet()
        {
            SortByColumn = SortBy.EmailAddress;
            SortByState = SortDirection.ASC;
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            await RefreshPage();
        }

        private string GetSortDirectionIcon(SortDirection sortDirection)
        {
            switch (sortDirection)
            {
                case SortDirection.ASC:
                    return "&nbsp;&uarr;";
                default:
                    return "&nbsp;&darr;";
            }
        }

        public async Task OnPostSortBy(SortBy sortby, SortDirection sortDirection)
        {
            ClearValidationState();
            SortByColumn = sortby;
            SortByState = sortDirection;
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            await RefreshPage();
        }

        public async Task OnPostSetUserAccountStatus(int accountstatususerid, int userselectedindex, int[] UserAccountStatusId)
        {
            ClearValidationState();
            var userUpdateStatus = new UserUpdateStatus()
            {
                UserSessionId = UserSessionId,
                UserId = accountstatususerid,
                UserAccountStatusId = UserAccountStatusId[userselectedindex],
                RequestUrl = FullUrl
            };

            await _userService.SetUserStatus(userUpdateStatus);
            await RefreshPage();
        }

        public async Task OnPostSetMultiSearch(int multisearchstatususerid, bool multisearchstatus)
        {
            ClearValidationState();

            var userUpdateMultiSearch = new UserUpdateMultiSearch()
            {
                UserSessionId = UserSessionId,
                UserId = multisearchstatususerid,
                MultiSearchEnabled = multisearchstatus,
                RequestUrl = FullUrl
            };

            await _userService.SetMultiSearch(userUpdateMultiSearch);
            await RefreshPage();
        }

        public async Task OnPostSetIsAdmin(int isadminuserid, bool isadmin)
        {
            ClearValidationState();

            var userUpdateIsAdmin = new UserUpdateIsAdmin()
            {
                UserSessionId = UserSessionId,
                UserId = isadminuserid,
                IsAdmin = isadmin,
                RequestUrl = FullUrl
            };

            await _userService.SetIsAdmin(userUpdateIsAdmin);
            await RefreshPage();
        }

        public async Task OnPostSetOrgTypeSearch(int orgtypesearchstatususerid, bool orgtypesearchstatus)
        {
            ClearValidationState();

            var userUpdateOrgTypeSearch = new UserUpdateOrgTypeSearch()
            {
                UserSessionId = UserSessionId,
                UserId = orgtypesearchstatususerid,
                OrgTypeSearchEnabled = orgtypesearchstatus,
                RequestUrl = FullUrl
            };

            await _userService.SetOrgTypeSearch(userUpdateOrgTypeSearch);
            await RefreshPage();

        }

        private void ClearValidationState()
        {
            ModelState.ClearValidationState("UserEmailAddress");
        }

        public async Task OnPostApplyFilter()
        {
            ClearValidationState();
            await RefreshPage();
        }

        public async Task OnPostSaveNewUser()
        {
            if (ModelState.IsValid)
            {
                var addUser = new AddUser()
                {
                    UserSessionId = UserSessionId,
                    EmailAddress = CleansedUserEmailAddress,
                    RequestUrl = FullUrl
                };
                await _userService.AddUserAsync(addUser);
                UserEmailAddress = null;
            }
            await RefreshPage();
        }

        public async Task OnPostClearSearch()
        {
            ClearValidationState();
            SurnameSearchValue = null;
            EmailAddressSearchValue = null;
            OrganisationNameSearchValue = null;
            SelectedAccessLevelFilter = null;
            SelectedMultiSearchFilter = null;
            SelectedOrgTypeSearchFilter = null;
            SelectedUserAccountStatusFilter = null;
            ModelState.Clear();
            await RefreshPage();
        }

        private async Task<IActionResult> RefreshPage()
        {
            var request = new UserListAdvanced
            {
                Surname = SurnameSearchValue,
                EmailAddress = EmailAddressSearchValue,
                SortByColumn = SortByColumn,
                SortDirection = SortByState,
                UserAccountStatusFilter = SelectedUserAccountStatusFilter,
                AccessLevelFilter = SelectedAccessLevelFilter,
                MultiSearchFilter = SelectedMultiSearchFilter,
                OrgTypeSearchFilter = SelectedOrgTypeSearchFilter
            };
            var userList = await _userService.GetUsersAsync(request);
            UserList = userList;
            return Page();
        }
    }
}