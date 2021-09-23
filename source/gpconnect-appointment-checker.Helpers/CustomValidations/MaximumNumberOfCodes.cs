using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers.CustomValidations
{
    public class MaximumNumberOfCodesAttribute : ValidationAttribute
    {
        public string _customErrorMessage { get; }
        public string _multiSearchErrorMessage { get; }
        private IHttpContextAccessor _httpContextAccessor { get; set; }
        private string _dependentProperty;

        public MaximumNumberOfCodesAttribute(string dependentProperty, string customErrorMessage, string multiSearchErrorMessage)
        {
            _dependentProperty = dependentProperty;
            _customErrorMessage = customErrorMessage;
            _multiSearchErrorMessage = multiSearchErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            _httpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var multiSearchEnabled = _httpContextAccessor?.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false);
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(this._dependentProperty);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult($"Unknown Property {_dependentProperty}");
            }

            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                var valueElements = value.ToString()?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray().Length;
                if (valueElements > (int)propertyTestedValue)
                {
                    var validationErrorMessage = multiSearchEnabled.GetValueOrDefault() ? string.Format(_customErrorMessage, (int)propertyTestedValue) : _multiSearchErrorMessage;
                    return new ValidationResult(validationErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }

}
