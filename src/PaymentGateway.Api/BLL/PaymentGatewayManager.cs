
using System.Text.Json;
using PaymentGateway.Api.DAL;
using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.BLL
{
    /*
        Inject HTTP Factory to avoid having to create a new HTTP client on each request being processed
        which would be inefficient
    */
    public sealed class PaymentGatewayManager(IHttpClientFactory httpClientFactory, IPaymentGatewayDal dal)
        : IPaymentGatewayManager
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BankClient");

        private readonly IPaymentGatewayDal _dal = dal;

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            try
            {
                var mappedObject = MapRequest(paymentRequest);

                // Optionally save request as mappedObject for auditing here

                var jsonContent = JsonSerializer.Serialize(mappedObject);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("payments", httpContent);

                // Assume Rejected response initially for safety
                var paymentResponse = new PaymentResponse
                {
                    Id = Guid.Empty,
                    Status = Enums.PaymentStatus.Rejected,
                    ExpiryMonth = paymentRequest.ExpiryMonth,
                    ExpiryYear = paymentRequest.ExpiryYear,
                    Currency = paymentRequest.Currency,
                    Amount = paymentRequest.Amount,
                    LastFourCardDigits = paymentRequest.CardNumber[^4..]
                };

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var bankApiResponse = JsonSerializer.Deserialize<BankResponse>(responseContent);

                    paymentResponse.Status = bankApiResponse.Authorized ? Enums.PaymentStatus.Authorized : Enums.PaymentStatus.Declined;

                    // save response
                    return await _dal.SavePaymentResponseAsync(paymentResponse);
                }
                else
                {
                    // Log the payment for audit purposes in a DB or some data storage
                    // Return rejected if no success response from the bank Api
                    return paymentResponse;
                }
            }
            catch (Exception)
            {
                // TODO: Process and push to cloud infra logging
                throw;
            }
        }

        public async Task<PaymentResponse?> FetchPaymentAsync(Guid paymentId)
        {
            try
            {
                return await _dal.FetchPaymentResponseAsync(paymentId);
            }
            catch (Exception)
            {
                // TODO: Process and push to cloud infra logging
                throw;
            }
        }

        private static BankRequest MapRequest(PaymentRequest paymentRequest)
        {
            return new BankRequest()
            {
                Card_Number = paymentRequest.CardNumber,
                Expiry_Date = $"{(paymentRequest.ExpiryMonth < 10 ? "0" : "")}{paymentRequest.ExpiryMonth}/{paymentRequest.ExpiryYear}",
                Currency = paymentRequest.Currency,
                Amount = paymentRequest.Amount,
                Cvv = paymentRequest.Cvv
            };
        }
    }
}