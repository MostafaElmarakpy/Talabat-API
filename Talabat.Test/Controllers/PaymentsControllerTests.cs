using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

using Stripe;
using Talabat.Controllers;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order_Aggregate;
using Talabat.Core.Service;
using Talabat.Dtos;
using Xunit;

namespace Talabat.Tests.Controllers

{
    public class PaymentsControllerTests
    {
        private readonly Mock<IPaymentService> _mockPaymentService;
        private readonly Mock<ILogger<PaymentsController>> _mockLogger;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly PaymentsController _controller;

        public PaymentsControllerTests()
        {
             //Arrange - Common test setup
            _mockPaymentService = new Mock<IPaymentService>();
            _mockLogger = new Mock<ILogger<PaymentsController>>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(x => x["StripeSetting:WebhookSecret"])
                .Returns("test_webhook_secret");

            _controller = new PaymentsController(
                _mockPaymentService.Object,
                _mockLogger.Object,
                _mockConfiguration.Object
            );
        }

        [Fact]
        public async Task CreateAndUpdatePaymentIntent_ValidBasketId_ReturnsOkResult()
        {
            // Arrange
            var basketId = "test-basket-123";

            var customerBasket = new CustomerBasket(basketId);

            _mockPaymentService
                .Setup(x => x.CreateOrUpdatePaymentIntent(basketId))
                .ReturnsAsync(customerBasket);

            // Act
            var result = await _controller.CreateAndUpdatePaymentIntent(basketId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<CustomerBasket>(okResult.Value);
            Assert.Equal(basketId, returnValue.Id);
        }

        [Fact]
        public async Task CreateAndUpdatePaymentIntent_InvalidBasketId_ReturnsBadRequest()
        {
            // Arrange
            var basketId = "invalid-basket";

            _mockPaymentService
                .Setup(x => x.CreateOrUpdatePaymentIntent(basketId))
                .ReturnsAsync((CustomerBasket)null);

            // Act
            var result = await _controller.CreateAndUpdatePaymentIntent(basketId);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Contains("Problem with your Basket", badRequestResult.Value.ToString());
        }




        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("invalid-basket-id")]
        public async Task CreateAndUpdatePaymentIntent_EdgeCases_HandlesGracefully(string basketId)
        {
            // Arrange
            _mockPaymentService
              .Setup(x => x.CreateOrUpdatePaymentIntent(basketId))
              .ReturnsAsync((CustomerBasket?)null);

            // Act
            var result = await _controller.CreateAndUpdatePaymentIntent(basketId);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task WebHook_InvalidSignature_ThrowsStripeException()
        {
            // Arrange
            _mockConfiguration
                .Setup(x => x["StripeSetting:WebhookSecret"])
                .Returns("invalid_secret");

            // Act & Assert
            await Assert.ThrowsAsync<StripeException>(() => _controller.WebHook());
        }
    }
}