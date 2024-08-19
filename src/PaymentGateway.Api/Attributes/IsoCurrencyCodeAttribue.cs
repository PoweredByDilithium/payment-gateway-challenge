using System.ComponentModel.DataAnnotations;

namespace PaymentGateway.Api.Attributes
{
    public class IsoCurrencyCodeAttribute : ValidationAttribute
    {
        private static readonly HashSet<string> IsoCurrencyCodes =
        [
            "USD", "EUR", "GBP"
        ];

        public IsoCurrencyCodeAttribute()
        {
            ErrorMessage = "The {0} must be a valid ISO 4217 currency code.";
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null || value is not string currencyCode)
            {
                return new ValidationResult(ErrorMessage);
            }

            if (!IsoCurrencyCodes.Contains(currencyCode.ToUpperInvariant()))
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}
