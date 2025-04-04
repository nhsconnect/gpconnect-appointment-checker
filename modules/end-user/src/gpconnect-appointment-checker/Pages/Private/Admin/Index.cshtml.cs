using System.Collections.Generic;
using GpConnect.AppointmentChecker.Core.Configuration;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Threading.Tasks;
using gpconnect_appointment_checker.Core.HttpClientServices.Interfaces;
using gpconnect_appointment_checker.Models;


namespace gpconnect_appointment_checker.Pages
{
    public partial class AdminModel : BaseModel
    {
        private readonly IUserService _userService;

        [BindProperty] public PagingModel Paging { get; set; }

        public AdminModel(IUserService userService, IOptions<GeneralConfig> configuration,
            IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {
            _userService = userService;
            FilterModel = new AdminFilterModel();
            Paging = new PagingModel(0, 0, (page) =>
            {
                var routeValues = new Dictionary<string, string>
                {
                    { "pageNumber", page.ToString() },
                    { "Filter.EmailAddress", FilterModel.EmailAddress },
                    { "Filter.Surname", FilterModel.Surname },
                    { "Filter.AccessLevelFilter", FilterModel.AccessLevelFilter.ToString() },
                    { "Filter.MultiSearchFilter", FilterModel.MultiSearchFilter.ToString().ToLower() },
                    { "Filter.OrgTypeSearchFilter", FilterModel.OrgTypeSearchFilter.ToString().ToLower() },
                    { "Filter.UserAccountStatusFilter", FilterModel.UserAccountStatusFilter.ToString() }
                };
                return routeValues;
            });
        }


        public async Task OnGet(int pageNumber = 1)
        {
            Paging.CurrentPage = pageNumber;
            SortByColumn = SortBy.EmailAddress;
            SortByState = SortDirection.Asc;
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            await LoadPage(FilterModel);
        }

        public async Task OnPostFilter(int pageNumber = 1)
        {
            ClearValidationState();
            Paging.CurrentPage = pageNumber;
            SortByColumn = SortBy.EmailAddress;
            SortByState = SortDirection.Asc;
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);

            await LoadPage(FilterModel);
        }

        private string GetSortDirectionIcon(SortDirection sortDirection)
        {
            return sortDirection switch
            {
                SortDirection.Asc => "&nbsp;&uarr;",
                _ => "&nbsp;&darr;"
            };
        }

        public async Task OnPostSortBy(SortBy sortby, SortDirection sortDirection)
        {
            ClearValidationState();
            SortByColumn = sortby;
            SortByState = sortDirection;
            SortByDirectionIcon = GetSortDirectionIcon(SortByState);
            Paging = new PagingModel(0, 0, (page) =>
            {
                var routeValues = new Dictionary<string, string>
                {
                    { "pageNumber", page.ToString() },
                    { "Filter.EmailAddress", FilterModel.EmailAddress },
                    { "Filter.Surname", FilterModel.Surname },
                    { "Filter.AccessLevelFilter", FilterModel.AccessLevelFilter.ToString() },
                    { "Filter.MultiSearchFilter", FilterModel.MultiSearchFilter.ToString().ToLower() },
                    { "Filter.OrgTypeSearchFilter", FilterModel.OrgTypeSearchFilter.ToString().ToLower() },
                    { "Filter.UserAccountStatusFilter", FilterModel.UserAccountStatusFilter.ToString() }
                };
                return routeValues;
            });
            await LoadPage(FilterModel);
        }

        public async Task OnPostClearSearch()
        {
            ClearValidationState();
            FilterModel.Surname = null;
            FilterModel.EmailAddress = null;
            FilterModel.AccessLevelFilter = null;
            FilterModel.MultiSearchFilter = null;
            FilterModel.OrgTypeSearchFilter = null;
            FilterModel.UserAccountStatusFilter = null;

            // Reinitialize Paging to make sure GetRouteValues is set again
            Paging = new PagingModel(1, Paging.TotalPages, (page) =>
            {
                var routeValues = new Dictionary<string, string>
                {
                    { "pageNumber", page.ToString() },
                    { "Filter.EmailAddress", FilterModel.EmailAddress },
                    { "Filter.Surname", FilterModel.Surname },
                    { "Filter.AccessLevelFilter", FilterModel.AccessLevelFilter.ToString() },
                    { "Filter.MultiSearchFilter", FilterModel.MultiSearchFilter.ToString().ToLower() },
                    { "Filter.OrgTypeSearchFilter", FilterModel.OrgTypeSearchFilter.ToString().ToLower() },
                    { "Filter.UserAccountStatusFilter", FilterModel.UserAccountStatusFilter.ToString() }
                };
                return routeValues;
            });
            ModelState.Clear();
            await LoadPage(FilterModel);
        }

        public async Task OnPostSetUserAccountStatus(int accountStatusUserId, int userSelectedIndex,
            int[] userAccountStatusId)
        {
            ClearValidationState();
            var userUpdateStatus = new UserUpdateStatus()
            {
                UserSessionId = UserSessionId,
                UserId = accountStatusUserId,
                UserAccountStatusId = userAccountStatusId[userSelectedIndex],
                RequestUrl = FullUrl
            };

            await _userService.SetUserStatus(userUpdateStatus);
            await LoadPage(FilterModel);
        }

        public async Task OnPostSetMultiSearch(int multiSearchStatusUserId, bool multiSearchStatus)
        {
            ClearValidationState();

            var userUpdateMultiSearch = new UserUpdateMultiSearch()
            {
                UserSessionId = UserSessionId,
                UserId = multiSearchStatusUserId,
                MultiSearchEnabled = multiSearchStatus,
                RequestUrl = FullUrl
            };

            await _userService.SetMultiSearch(userUpdateMultiSearch);
            await LoadPage(FilterModel);
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
            await LoadPage(FilterModel);
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
            await LoadPage(FilterModel);
        }

        private void ClearValidationState()
        {
            ModelState.ClearValidationState("UserEmailAddress");
        }

        public async Task OnGetRefresh()
        {
            await LoadPage(FilterModel);
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
                UserEmailAddress = "";
            }

            await LoadPage(FilterModel);
        }


        private async Task<IActionResult> LoadPage(AdminFilterModel filter)
        {
            var request = new UserListAdvanced
            {
                Surname = filter.Surname,
                EmailAddress = filter.EmailAddress,
                SortByColumn = SortByColumn,
                SortDirection = SortByState,
                UserAccountStatusFilter = filter.UserAccountStatusFilter,
                AccessLevelFilter = filter.AccessLevelFilter,
                MultiSearchFilter = filter.MultiSearchFilter,
                OrgTypeSearchFilter = filter.OrgTypeSearchFilter
            };

            var pagedResult = await _userService.GetUsersAsync(request, Paging.CurrentPage);

            UserList = pagedResult.Items;
            Paging.PageSize = 50;
            Paging.TotalItems = pagedResult.TotalItems;
            Paging.TotalPages = pagedResult.TotalPages;
            Paging.GetRouteValues = (page) =>
            {
                var routeValues = new Dictionary<string, string>
                {
                    { "pageNumber", page.ToString() },
                    { "Filter.EmailAddress", FilterModel.EmailAddress },
                    { "Filter.Surname", FilterModel.Surname },
                    { "Filter.AccessLevelFilter", FilterModel.AccessLevelFilter.ToString() },
                    { "Filter.MultiSearchFilter", FilterModel.MultiSearchFilter.ToString().ToLower() },
                    { "Filter.OrgTypeSearchFilter", FilterModel.OrgTypeSearchFilter.ToString().ToLower() },
                    { "Filter.UserAccountStatusFilter", FilterModel.UserAccountStatusFilter.ToString() }
                };
                return routeValues;
            };
            return Page();
        }
    }
}