namespace PaymentGateway.Api.Models
{
    public sealed class BankRequest
    {

        public required string Card_Number { get; set; }

        public required string Expiry_Date { get; set; }

        public required string Currency { get; set; }

        public required int Amount { get; set; }

        public required string Cvv { get; set; }
    }
}