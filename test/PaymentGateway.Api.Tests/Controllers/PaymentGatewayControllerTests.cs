using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using PaymentGateway.Api.BLL;
using PaymentGateway.Api.Controllers;
using PaymentGateway.Api.Entities;
using Xunit;

namespace PaymentGateway.Api.Tests.Controllers
{
    public sealed class PaymentGatewayControllerTests
    {
        private readonly PaymentGatewayController _controller;
        private readonly Mock<IPaymentGatewayManager> _paymentGatewayManagerMock;

        public PaymentGatewayControllerTests()
        {
            _paymentGatewayManagerMock = new Mock<IPaymentGatewayManager>();
            _controller = new PaymentGatewayController(_paymentGatewayManagerMock.Object);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnOkResult_WhenRequestIsValid()
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

            var paymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                Status = Enums.PaymentStatus.Authorized,
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                LastFourCardDigits = paymentRequest.CardNumber[^4..],

            };

            _paymentGatewayManagerMock
                .Setup(m => m.ProcessPaymentAsync(paymentRequest))
                .ReturnsAsync(paymentResponse);

            // Act
            var result = await _controller.Post(paymentRequest) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.OK, result?.StatusCode);
            Assert.Equal(paymentResponse, result?.Value);
        }

        [Fact]
        public async Task ProcessPayment_ShouldReturnBadRequestError_WhenExceptionIsThrown()
        {
            // Arrange
            var paymentRequest = new PaymentRequest
            {
                CardNumber = "2222",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100,
                Cvv = "123"
            };

            _paymentGatewayManagerMock
                .Setup(m => m.ProcessPaymentAsync(paymentRequest))
                .ThrowsAsync(new Exception("Bad Request"));

            // Act
            var result = await _controller.Post(paymentRequest) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, result?.StatusCode);
        }

        [Fact]
        public async Task Get_ReturnsOkResult_WithPaymentResponse()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedPaymentResponse = new PaymentResponse
            {
                Id = paymentId,
                Status = Enums.PaymentStatus.Authorized,
                ExpiryMonth = 12,
                ExpiryYear = DateTime.Now.Year + 1,
                Currency = "USD",
                Amount = 100,
                LastFourCardDigits = "1234"
            };

            _paymentGatewayManagerMock.Setup(m => m.FetchPaymentAsync(paymentId))
                .ReturnsAsync(expectedPaymentResponse);

            // Act
            var result = await _controller.Get(paymentId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result?.StatusCode);
            Assert.Equal(expectedPaymentResponse, result?.Value);

            _paymentGatewayManagerMock.Verify(m => m.FetchPaymentAsync(paymentId), Times.Once);
        }

        [Fact]
        public async Task Get_ReturnsBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var exceptionMessage = "An error occurred";
            
            _paymentGatewayManagerMock.Setup(m => m.FetchPaymentAsync(paymentId))
                .ThrowsAsync(new Exception(exceptionMessage));

            // Act
            var result = await _controller.Get(paymentId) as BadRequestObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result?.StatusCode);
            Assert.Equal(exceptionMessage, ((Exception)result.Value).Message);

            _paymentGatewayManagerMock.Verify(m => m.FetchPaymentAsync(paymentId), Times.Once);
        }
    }
}
