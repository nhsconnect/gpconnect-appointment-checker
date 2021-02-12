using Microsoft.Extensions.Configuration;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers.CustomValidations
{
    public class MaximumNumberOfCodesAttribute : ValidationAttribute
    {
        public string _configurationSetting { get; }
        public string _customErrorMessage { get; }
        public int _defaultValue { get; }
        private IConfiguration _configuration { get; set; }

        public MaximumNumberOfCodesAttribute(string configurationSetting, string customErrorMessage, int defaultValue)
        {
            _configurationSetting = configurationSetting;
            _customErrorMessage = customErrorMessage;
            _defaultValue = defaultValue;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _configuration = (IConfiguration)validationContext.GetService(typeof(IConfiguration));

            var maximumNumberOfCodesSetting = _configuration[$"General:{_configurationSetting}"].StringToInteger(_defaultValue);
            if (!string.IsNullOrEmpty(value.ToString()))
            {
                var valueElements = value.ToString()?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x))
                    .ToArray().Length;
                if (valueElements > maximumNumberOfCodesSetting)
                {
                    return new ValidationResult(string.Format(_customErrorMessage, maximumNumberOfCodesSetting));
                }
            }
            return ValidationResult.Success;
        }
    }

}
