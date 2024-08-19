using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.BLL
{
    public interface IPaymentGatewayManager
    {
        public Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest);

        public Task<PaymentResponse?> FetchPaymentAsync(Guid paymentId);
    }
}