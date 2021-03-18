using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers.CustomValidations
{
    public class MaximumNumberOfCodesAttribute : ValidationAttribute
    {
        public string _configurationSetting { get; }
        public string _customErrorMessage { get; }
        public string _multiSearchErrorMessage { get; }
        public int _defaultValue { get; }
        private IConfiguration _configuration { get; set; }
        private IHttpContextAccessor _httpContextAccessor { get; set; }

        public MaximumNumberOfCodesAttribute(string configurationSetting, string customErrorMessage, string multiSearchErrorMessage, int defaultValue)
        {
            _configurationSetting = configurationSetting;
            _customErrorMessage = customErrorMessage;
            _multiSearchErrorMessage = multiSearchErrorMessage;
            _defaultValue = defaultValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));
            _httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var multiSearchEnabled = _httpContextAccessor?.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false);
            var maximumNumberOfCodesSetting = !multiSearchEnabled.GetValueOrDefault() ? 1 : _configuration[$"General:{_configurationSetting}"].StringToInteger(_defaultValue);

            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                var valueElements = value.ToString()?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray().Length;
                if (valueElements > maximumNumberOfCodesSetting)
                {
                    var validationErrorMessage = multiSearchEnabled.GetValueOrDefault() ? string.Format(_customErrorMessage, maximumNumberOfCodesSetting) : _multiSearchErrorMessage;
                    return new ValidationResult(validationErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }

}
