using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.DAL
{
    public interface IPaymentGatewayDal
    {
        public Task<PaymentRequest> SavePaymentRequestAsync(PaymentRequest paymentRequest);

        public Task<PaymentResponse> SavePaymentResponseAsync(PaymentResponse paymentResponse);

        public Task<PaymentResponse?> FetchPaymentResponseAsync(Guid paymentId);
    }
}