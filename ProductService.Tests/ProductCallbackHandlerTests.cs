using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using NServiceBus;
using NServiceBus.Callbacks.Testing;
using ReadingIsGood.ProductService.Contracts.Messages;
using ReadingIsGood.ProductService.Entities;
using ReadingIsGood.Shared;
using Xunit;

namespace ProductService.Tests
{
    public class ProductCallbackHandlerTests : IClassFixture<TestFixture>
    {
        private readonly TestFixture _testFixture;

        public ProductCallbackHandlerTests(TestFixture testFixture)
        {
            _testFixture = testFixture;
        }

        [Fact]
        [Trait("ProductCallback","SaveProduct_CallbackTest")]
        public async Task SaveProduct_CallbackTest()
        {
            var request = new SaveProductRequest("test2", 10, 12, Guid.NewGuid());

            var simulatedResponse = CallbackResult<string>.SuccessResult("Product created");

            var session = new TestableCallbackAwareSession();
            session.When(
                matcher: (SaveProductRequest message) => message == request,
                response: simulatedResponse);
            
            var result = await session.Request<CallbackResult<string>>(request)
                .ConfigureAwait(false);

            result.Payload.Should().Be(simulatedResponse.Payload);
        }
        
        [Fact]
        [Trait("ProductCallback","UpdateStock_CallbackTest")]
        public async Task UpdateStock_CallbackTest()
        {
            var product = new Product()
            {
                Id = Guid.NewGuid(),
                Name = "test3",
                Price = 15,
                Stock = 164,
                AuditInformation = new AuditInformation()
                {
                    CreatedBy = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow
                }
            };

            await _testFixture.ProductRepository.SaveProduct(product);

            var request = new UpdateStockRequest(product.Id, product.AuditInformation.CreatedBy, -100);

            var simulatedResponse = CallbackResult<string>.SuccessResult("Stock updated");

            var session = new TestableCallbackAwareSession();
            session.When(
                matcher: (UpdateStockRequest message) => message == request,
                response: simulatedResponse);
            
            var result = await session.Request<CallbackResult<string>>(request)
                .ConfigureAwait(false);

            result.Payload.Should().Be(simulatedResponse.Payload);
        }
    }
}