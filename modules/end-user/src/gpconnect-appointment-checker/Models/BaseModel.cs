using GpConnect.AppointmentChecker.Core.Configuration;
using gpconnect_appointment_checker.Helpers;
using gpconnect_appointment_checker.Helpers.Enumerations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using System;
using System.IO;

namespace gpconnect_appointment_checker.Pages
{
    public abstract class BaseModel : PageModel
    {
        private readonly IOptions<GeneralConfig> _generalOptionsDelegate;
        private readonly IHttpContextAccessor _contextAccessor;

        protected BaseModel(IOptions<GeneralConfig> generalOptionsDelegate, IHttpContextAccessor contextAccessor)
        {
            _generalOptionsDelegate = generalOptionsDelegate;
            _contextAccessor = contextAccessor;
        }

        public string GetAccessEmailAddress => _generalOptionsDelegate.Value.GetAccessEmailAddress;
        public string GetAccessEmailAddressLink => $"<a href=\"mailto:{_generalOptionsDelegate.Value.GetAccessEmailAddress}\">{_generalOptionsDelegate.Value.GetAccessEmailAddress}</a>";
        public int MaxNumberProviderCodesSearch => _generalOptionsDelegate.Value.MaxNumberProviderCodesSearch;
        public int MaxNumberConsumerCodesSearch => _generalOptionsDelegate.Value.MaxNumberConsumerCodesSearch;
        public int MaxNumberWeeksSearch => _generalOptionsDelegate.Value.MaxNumWeeksSearch;
        public string LastUpdated => $"{DateTime.UtcNow:MMMM yyyy}";
        public string ApplicationName => _generalOptionsDelegate.Value.ProductName;
        public bool MultiSearchEnabled => _contextAccessor.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false);
        public bool OrgTypeSearchEnabled => _contextAccessor.HttpContext.User.GetClaimValue("OrgTypeSearchEnabled").StringToBoolean(false);
        public bool UserIsAdmin => _contextAccessor.HttpContext.User.GetClaimValue("IsAdmin").StringToBoolean(false);
        public int UserId => _contextAccessor.HttpContext.User.GetClaimValue("UserId").StringToInteger(0);
        public string Sid => _contextAccessor.HttpContext.User.GetClaimValue("sid");

        public bool NoUserPresent => UserId == 0;
        public UserAccountStatus UserAccountStatus => GetUserAccountStatus(_contextAccessor.HttpContext.User.GetClaimValue<UserAccountStatus>("UserAccountStatus"));

        public Uri FullUrl => new Uri(HttpContext.Request.GetDisplayUrl());

        private UserAccountStatus GetUserAccountStatus(UserAccountStatus? userAccountStatus)
        {
            if (userAccountStatus.HasValue)
            {
                return userAccountStatus.Value;
            }
            return UserAccountStatus.Unknown;
        }

        protected static FileStreamResult GetFileStream(MemoryStream memoryStream, string fileName = null)
        {
            return new FileStreamResult(memoryStream, new MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"))
            {
                FileDownloadName = fileName ?? $"{DateTime.UtcNow.ToFileTimeUtc()}.xlsx"
            };
        }
    }
}