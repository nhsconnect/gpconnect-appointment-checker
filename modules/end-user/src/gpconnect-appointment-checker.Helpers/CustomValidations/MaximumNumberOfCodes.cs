using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;

using gpconnect_appointment_checker.Helpers.Extensions;

namespace gpconnect_appointment_checker.Helpers.CustomValidations
{
    public class MaximumNumberOfCodesAttribute : ValidationAttribute
    {
        public string CustomErrorMessage { get; }
        public string MultiSearchErrorMessage { get; }
        private IHttpContextAccessor HttpContextAccessor { get; set; }
        private string _dependentProperty;

        public MaximumNumberOfCodesAttribute(string dependentProperty, string customErrorMessage, string multiSearchErrorMessage)
        {
            _dependentProperty = dependentProperty;
            CustomErrorMessage = customErrorMessage;
            MultiSearchErrorMessage = multiSearchErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            HttpContextAccessor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));
            var multiSearchEnabled = (HttpContextAccessor?.HttpContext.User.GetClaimValue("MultiSearchEnabled").StringToBoolean(false)).GetValueOrDefault();
            var propertyTestedInfo = validationContext.ObjectType.GetProperty(_dependentProperty);
            if (propertyTestedInfo == null)
            {
                return new ValidationResult($"Unknown Property {_dependentProperty}");
            }

            if (!string.IsNullOrEmpty(value?.ToString()))
            {
                var propertyTestedValue = propertyTestedInfo.GetValue(validationContext.ObjectInstance, null);

                var valueElements = value.ToString()?.Split(',', ' ').Where(x => !string.IsNullOrWhiteSpace(x)).ToArray().Length;

                if(!multiSearchEnabled && valueElements > 1)
                {
                    return new ValidationResult(MultiSearchErrorMessage);
                }

                if (valueElements > (int)propertyTestedValue)
                {
                    var validationErrorMessage = multiSearchEnabled ? string.Format(CustomErrorMessage, (int)propertyTestedValue) : MultiSearchErrorMessage;
                    return new ValidationResult(validationErrorMessage);
                }
            }
            return ValidationResult.Success;
        }
    }

}
