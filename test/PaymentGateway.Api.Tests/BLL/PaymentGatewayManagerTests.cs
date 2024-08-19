using System.Net;
using System.Text;
using System.Text.Json;

using Moq;
using Moq.Protected;
using PaymentGateway.Api.BLL;
using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Models;

namespace PaymentGateway.Api.Tests.BLL
{
    public class PaymentGatewayManagerTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
        private readonly HttpClient _httpClient;
        private readonly PaymentGatewayManager _paymentGatewayManager;

        public PaymentGatewayManagerTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>();
            _httpClient = new HttpClient(_httpMessageHandlerMock.Object)
            {
                BaseAddress = new Uri("http://localhost:8080/")
            };

            _httpClientFactoryMock = new Mock<IHttpClientFactory>();
            _httpClientFactoryMock.Setup(factory => factory.CreateClient("BankClient"))
                                  .Returns(_httpClient);

            _paymentGatewayManager = new PaymentGatewayManager(_httpClientFactoryMock.Object);
        }

        [Fact]
        public async Task ProcessPaymentAsync_AuthorizedResponse_ReturnsAuthorizedPaymentResponse()
        {
            // Arrange
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "2222405343248877",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Cvv = "123"
            };

            var bankApiResponse = new BankResponse()
            {
                Authorized = true,
                Authorization_Code = Guid.NewGuid()
            };

            var responseContent = JsonSerializer.Serialize(bankApiResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var actualResponse = await _paymentGatewayManager.ProcessPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(Enums.PaymentStatus.Authorized, actualResponse.Status);
            Assert.Equal(paymentRequest.Currency, actualResponse.Currency);
            Assert.Equal(paymentRequest.Amount, actualResponse.Amount);
            Assert.Equal("8877", actualResponse.LastFourCardDigits);
        }

        [Fact]
        public async Task ProcessPaymentAsync_NotAuthorizedResponse_ReturnsDeclinedPaymentResponse()
        {
            // Arrange
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "2222405343248112",
                ExpiryMonth = 1,
                ExpiryYear = 2026,
                Currency = "USD",
                Amount = 60000,
                Cvv = "456"
            };

            var bankApiResponse = new BankResponse()
            {
                Authorized = false,
                Authorization_Code = Guid.Empty
            };

            var responseContent = JsonSerializer.Serialize(bankApiResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var actualResponse = await _paymentGatewayManager.ProcessPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(Enums.PaymentStatus.Declined, actualResponse.Status);
            Assert.Equal(paymentRequest.Currency, actualResponse.Currency);
            Assert.Equal(paymentRequest.Amount, actualResponse.Amount);
            Assert.Equal("8112", actualResponse.LastFourCardDigits);
        }

        [Fact]
        public async Task ProcessPaymentAsync_RejectedResponse_ReturnsRejectedResponse()
        {
            // Arrange
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "1234",
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = "GBP",
                Amount = 100,
                Cvv = "123"
            };

            var bankApiResponse = new BankResponse()
            {
                Authorized = false,
                Authorization_Code = Guid.NewGuid()
            };

            var responseContent = JsonSerializer.Serialize(bankApiResponse);

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent(responseContent, Encoding.UTF8, "application/json")
                });

            // Act
            var actualResponse = await _paymentGatewayManager.ProcessPaymentAsync(paymentRequest);

            // Assert
            Assert.Equal(Enums.PaymentStatus.Rejected, actualResponse.Status);
        }
    }
}
