using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.ProductService.Contracts.Messages;
using ReadingIsGood.ProductService.Entities;
using ReadingIsGood.ProductService.Repositories;
using ReadingIsGood.Shared;

namespace ReadingIsGood.ProductService.Handlers.CallbackHandlers
{
    public class ProductCallbackHandler :
        IHandleMessages<UpdateStockRequest>,
        IHandleMessages<SaveProductRequest>
    {
        public IProductRepository ProductRepository { get; set; }
        public ILogger Logger { get; set; }

        public async Task Handle(SaveProductRequest message, IMessageHandlerContext context)
        {
            var result = await ProductRepository.SaveProduct(new Product
            {
                Id = Guid.NewGuid(),
                Name = message.ProductName,
                Price = message.Price,
                Stock = message.Stock,
                AuditInformation = new AuditInformation
                    {CreatedBy = message.CreatedUserId, CreatedDate = DateTime.UtcNow}
            });

            if (result == 1)
            {
                Logger.LogInformation(
                    $"Product Created by User:{message.CreatedUserId} , ProductName:{message.ProductName}  Stock:{message.Stock}  Price:{message.Stock}");
                await context.Reply(CallbackResult<string>.SuccessResult("Product created"));
                return;
            }

            await context.Reply(CallbackResult<string>.ErrorResult("Failed", " SaveProduct Failed"));
        }

        public async Task Handle(UpdateStockRequest message, IMessageHandlerContext context)
        {
            var updateResult = await ProductRepository.UpdateStock(message.ProductId, message.Stock);

            if (updateResult == 1)
            {
                Logger.LogInformation(
                    $"User:{message.UpdatedById} updated stock that {message.Stock} this product:{message.ProductId}");

                await context.Reply(CallbackResult<string>.SuccessResult("Stock updated"));
                return;
            }

            await context.Reply(CallbackResult<string>.ErrorResult("Failed", " UpdateStock Failed"));
        }
    }
}