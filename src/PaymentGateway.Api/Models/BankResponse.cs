namespace PaymentGateway.Api.Models
{
    public sealed class BankResponse
    {
        public bool Authorized { get; set; }

        public Guid Authorization_Code { get; set; }
    }
}