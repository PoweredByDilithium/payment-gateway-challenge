using System.Net;
using System.Text;
using System.Text.Json;
using Moq;
using Moq.Protected;
using PaymentGateway.Api.BLL;
using PaymentGateway.Api.Entities;
using PaymentGateway.Api.Models;
using Microsoft.EntityFrameworkCore;
using PaymentGateway.Api.Database;
using PaymentGateway.Api.DAL;

namespace PaymentGateway.Api.Tests.DAL
{
    public class PaymentGatewayDalTests : IDisposable
    {
        private readonly DbContextOptions<Context> _options;
        private readonly Context _context;
        private readonly PaymentGatewayDal _dal;

        public PaymentGatewayDalTests()
        {
            _options = new DbContextOptionsBuilder<Context>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new Context(_options);
            _dal = new PaymentGatewayDal(_context);
        }

        // Dispose context after each test
        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        [Fact]
        public async Task FetchPaymentResponseAsync_ReturnsPaymentResponse_WhenExists()
        {
            // Arrange
            var paymentId = Guid.NewGuid();
            var expectedResponse = new PaymentResponse
            {   
                Id = paymentId,
                LastFourCardDigits = "1234",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100
            };
            await _context.PaymentResponses.AddAsync(expectedResponse);
            await _context.SaveChangesAsync();

            // Act
            var result = await _dal.FetchPaymentResponseAsync(paymentId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedResponse.Id, result.Id);
        }

        [Fact]
        public async Task FetchPaymentResponseAsync_ReturnsNull_WhenNotExists()
        {
            // Act
            var result = await _dal.FetchPaymentResponseAsync(Guid.NewGuid());

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task SavePaymentRequestAsync_SavesAndReturnsPaymentRequest()
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

            // Act
            var result = await _dal.SavePaymentRequestAsync(paymentRequest);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentRequest.CardNumber, result.CardNumber);
            Assert.Equal(paymentRequest.ExpiryMonth, result.ExpiryMonth);
            Assert.Equal(paymentRequest.ExpiryYear, result.ExpiryYear);
            Assert.Equal(paymentRequest.Currency, result.Currency);
            Assert.Equal(paymentRequest.Amount, result.Amount);
            Assert.Equal(paymentRequest.Cvv, result.Cvv);
        }

        [Fact]
        public async Task SavePaymentResponseAsync_SavesAndReturnsPaymentResponse()
        {
            // Arrange
            var paymentResponse = new PaymentResponse
            {
                Id = Guid.NewGuid(),
                LastFourCardDigits = "1234",
                ExpiryMonth = 4,
                ExpiryYear = 2025,
                Currency = "GBP",
                Amount = 100
            };

            // Act
            var result = await _dal.SavePaymentResponseAsync(paymentResponse);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(paymentResponse.Id, result.Id);
        }
    }
}
