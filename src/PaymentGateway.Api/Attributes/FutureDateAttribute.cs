using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Attributes
{
    public sealed class FutureDateAttribute : ValidationAttribute
    {
        private readonly string _yearPropertyName;

        public FutureDateAttribute(string yearPropertyName)
        {
            _yearPropertyName = yearPropertyName;
            ErrorMessage = "The combination of year and month must be in the future.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Get the month value
            if (value is not int month || month < 1 || month > 12)
            {
                return new ValidationResult("Invalid month value.");
            }

            // Get the year value using reflection
            var yearProperty = validationContext.ObjectType.GetProperty(_yearPropertyName);
            if (yearProperty == null)
            {
                return new ValidationResult($"Unknown property: {_yearPropertyName}");
            }

            var yearValue = yearProperty.GetValue(validationContext.ObjectInstance);
            if (yearValue is not int year)
            {
                return new ValidationResult("Invalid year value.");
            }

            // Construct a date from the year and month
            var dateToValidate = new DateTime(year, month, 1);

            // Compare with the current date
            if (dateToValidate <= DateTime.Now)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}