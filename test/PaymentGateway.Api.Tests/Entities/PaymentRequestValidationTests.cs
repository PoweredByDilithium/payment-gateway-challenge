using System.ComponentModel.DataAnnotations;
using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.Tests.Entities
{
    public sealed class PaymentRequestValidationTests
    {
        [Theory]
        [InlineData("1234567812345678", true)] // Valid
        [InlineData("12345678", false)] // Too short
        [InlineData("12345678123456789000", false)] // Too long
        [InlineData("1234abcd1234abcd", false)] // Contains non-numeric characters
        public void Test_CardNumberValidation(string cardNumber, bool expectedIsValid)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = cardNumber,
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = "USD",
                Amount = 100,
                Cvv = "123"
            };

            AssertValidationResult(paymentRequest, expectedIsValid);
        }

        [Theory]
        [InlineData(12, 2025, true)] // Valid future date
        [InlineData(1, 2100, true)] // Valid extreme future date
        [InlineData(0, 2025, false)] // Invalid month (too low)
        [InlineData(13, 2025, false)] // Invalid month (too high)
        [InlineData(12, 2020, false)] // Past date
        public void Test_ExpiryDateValidation(int expiryMonth, int expiryYear, bool expectedIsValid)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = expiryMonth,
                ExpiryYear = expiryYear,
                Currency = "USD",
                Amount = 100,
                Cvv = "123"
            };

            AssertValidationResult(paymentRequest, expectedIsValid);
        }

        [Theory]
        [InlineData("USD", true)] // Valid currency
        [InlineData("EUR", true)] // Valid currency
        [InlineData("US", false)] // Invalid length
        [InlineData("USDD", false)] // Invalid length
        [InlineData("XYZ", false)] // Invalid currency
        public void Test_CurrencyValidation(string currency, bool expectedIsValid)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = currency,
                Amount = 100,
                Cvv = "123"
            };

            AssertValidationResult(paymentRequest, expectedIsValid);
        }

        [Theory]
        [InlineData(100, true)] // Valid amount
        [InlineData(0, false)] // Invalid (zero amount)
        [InlineData(-50, false)] // Invalid (negative amount)
        public void Test_AmountValidation(int amount, bool expectedIsValid)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = "USD",
                Amount = amount,
                Cvv = "123"
            };

            AssertValidationResult(paymentRequest, expectedIsValid);
        }

        [Theory]
        [InlineData("123", true)] // Valid CVV (3 digits)
        [InlineData("1234", true)] // Valid CVV (4 digits)
        [InlineData("12", false)] // Invalid (too short)
        [InlineData("12345", false)] // Invalid (too long)
        [InlineData("12a4", false)] // Invalid (contains non-numeric characters)
        public void Test_CvvValidation(string cvv, bool expectedIsValid)
        {
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1234567812345678",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = "USD",
                Amount = 100,
                Cvv = cvv
            };

            AssertValidationResult(paymentRequest, expectedIsValid);
        }

        private static void AssertValidationResult(PaymentRequest paymentRequest, bool expectedIsValid)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(paymentRequest);
            bool isValid = Validator.TryValidateObject(paymentRequest, validationContext, validationResults, true);

            Assert.Equal(expectedIsValid, isValid);

            if (!isValid)
            {
                foreach (var validationResult in validationResults)
                {
                    Console.WriteLine(validationResult.ErrorMessage);
                }
            }
        }
    }
}
