using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Database;
using PaymentGateway.Api.Entities;

namespace PaymentGateway.Api.DAL
{
    public sealed class PaymentGatewayDal(Context context)
                : IPaymentGatewayDal
    {
        private readonly Context _context = context;

        public async Task<PaymentResponse?> FetchPaymentResponseAsync(Guid paymentId)
        {
            try
            {
                return await _context.PaymentResponses.SingleOrDefaultAsync(x => x.Id == paymentId);
            }
            catch (Exception)
            {
                // TODO: Process and push to cloud infra logging
                throw;
            }
        }


        public async Task<PaymentRequest> SavePaymentRequestAsync(PaymentRequest paymentRequest)
        {
            try
            {
                await _context.AddAsync( paymentRequest );
                await _context.SaveChangesAsync();
                return paymentRequest;
            }
            catch (Exception)
            {
                // TODO: Process and push to cloud infra logging
                throw;
            }
        }

        public async Task<PaymentResponse> SavePaymentResponseAsync(PaymentResponse paymentResponse)
        {
            try
            {
                await _context.AddAsync( paymentResponse );
                await _context.SaveChangesAsync();
                return paymentResponse;
            }
            catch (Exception)
            {
                // TODO: Process and push to cloud infra logging
                throw;
            }
        }

    }
}