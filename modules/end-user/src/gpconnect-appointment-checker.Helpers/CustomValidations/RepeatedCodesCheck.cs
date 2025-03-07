using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace gpconnect_appointment_checker.Helpers.CustomValidations
{
    public class RepeatedCodesCheckAttribute : ValidationAttribute
    {
        public string CustomErrorMessage { get; }

        public RepeatedCodesCheckAttribute(string customErrorMessage)
        {
            CustomErrorMessage = customErrorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(value?.ToString())) return ValidationResult.Success;

            var valueElements = value.ToString()?
                .Split(',', ' ')
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
            
            var duplicatedValueElements =
                valueElements.GroupBy(x => x).Where(g => g.Count() > 1).Select(y => y.Key).ToList();

            if (duplicatedValueElements.Count > 0)
            {
                return new ValidationResult(string.Format(CustomErrorMessage,
                    string.Join(", ", duplicatedValueElements)));
            }

            return ValidationResult.Success;
        }
    }
}