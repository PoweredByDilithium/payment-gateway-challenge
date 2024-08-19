
using System.Text.Json;

using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.BLL
{
    /*
        Inject HTTP Factory to avoid having to create a new HTTP client on each request being processed
    */
    public sealed class PaymentGatewayManager(IHttpClientFactory httpClientFactory)
        : IPaymentGatewayManager
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient("BankClient");

        public async Task<PaymentResponse> ProcessPaymentAsync(PaymentRequest paymentRequest)
        {
            try
            {
                var mappedObject = MapRequest(paymentRequest);
                var jsonContent = JsonSerializer.Serialize(mappedObject);
                var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("payments", httpContent);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var bankApiResponse = JsonSerializer.Deserialize<BankResponse>(responseContent);

                    return new PaymentResponse
                    {
                        Id = Guid.NewGuid(),
                        Status = bankApiResponse.Authorized ? Enums.PaymentStatus.Authorized : Enums.PaymentStatus.Declined,
                        ExpiryMonth = paymentRequest.ExpiryMonth,
                        ExpiryYear = paymentRequest.ExpiryYear,
                        Currency = paymentRequest.Currency,
                        Amount = paymentRequest.Amount,
                        LastFourCardDigits = paymentRequest.CardNumber[^4..]
                    };
                }
                else
                {
                    return new PaymentResponse
                    {
                        Id = Guid.Empty,
                        Status = Enums.PaymentStatus.Rejected,
                        ExpiryMonth = paymentRequest.ExpiryMonth,
                        ExpiryYear = paymentRequest.ExpiryYear,
                        Currency = paymentRequest.Currency,
                        Amount = paymentRequest.Amount,
                        LastFourCardDigits = paymentRequest.CardNumber[^4..]
                    };
                }
            }
            catch (Exception)
            {
                // Process and push to cloud infra logging
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