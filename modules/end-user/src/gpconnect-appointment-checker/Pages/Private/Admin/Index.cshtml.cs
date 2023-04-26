using GpConnect.AppointmentChecker.Core.Config;
using GpConnect.AppointmentChecker.Core.HttpClientServices.Interfaces;
using GpConnect.AppointmentChecker.Models.Request;
using gpconnect_appointment_checker.DTO.Response.Configuration;
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
        private readonly INotificationService _notificationService;
        private readonly IOptions<NotificationConfig> _notificationConfig;

        public AdminModel(IUserService userService, INotificationService notificationService, IOptions<NotificationConfig> notificationConfig, IOptions<General> configuration, IHttpContextAccessor contextAccessor) : base(configuration, contextAccessor)
        {
            _userService = userService;
            _notificationService = notificationService;
            _notificationConfig = notificationConfig;
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
            await _userService.SetUserStatus(accountstatususerid, UserAccountStatusId[userselectedindex]);

            //switch(UserAccountStatusId[userselectedindex])
            //{
            //    case (int)UserAccountStatus.Deauthorised:
            //        await _notificationService.PostNotificationAsync(new NotificationDetails
            //        {
            //            EmailAddresses = new System.Collections.Generic.List<string>() { user.EmailAddress },
            //            TemplateId = _notificationConfig.Value.AccountDeactivatedTemplateId
            //        });
            //        break;
            //    case (int)UserAccountStatus.Authorised:
            //        await _notificationService.PostNotificationAsync(new NotificationDetails
            //        {
            //            EmailAddresses = new System.Collections.Generic.List<string>() { user.EmailAddress },
            //            TemplateId = _notificationConfig.Value.NewAccountCreatedTemplateId
            //        });
            //        break;
            //}            

            await RefreshPage();
        }

        public async Task OnPostSetMultiSearch(int multisearchstatususerid, bool multisearchstatus)
        {
            ClearValidationState();
            await _userService.SetMultiSearch(multisearchstatususerid, multisearchstatus);
            await RefreshPage();
        }

        public async Task OnPostSetOrgTypeSearch(int orgtypesearchstatususerid, bool orgtypesearchstatus)
        {
            ClearValidationState();
            await _userService.SetOrgTypeSearch(orgtypesearchstatususerid, orgtypesearchstatus);
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
                UserEmailAddress = CleansedUserEmailAddress;
                var user = await _userService.AddUserAsync(UserEmailAddress);
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
                OrganisationName = OrganisationNameSearchValue,
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