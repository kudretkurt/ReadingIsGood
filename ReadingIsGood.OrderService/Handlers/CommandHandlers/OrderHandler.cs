using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.OrderService.Contracts.Commands;
using ReadingIsGood.OrderService.Entities;
using ReadingIsGood.OrderService.Repositories;

namespace ReadingIsGood.OrderService.Handlers.CommandHandlers
{
    public class OrderHandler : IHandleMessages<SaveOrderCommand>
    {
        public ILogger Logger { get; set; }
        public IMongoRepository<Order> OrderRepository { get; set; }

        public async Task Handle(SaveOrderCommand message, IMessageHandlerContext context)
        {
            try
            {
                var products = message.Products.Select(t => new Product
                {
                    Count = t.Count,
                    Name = t.Name,
                    Price = t.Price,
                    Id = t.Id.ToString()
                }).ToList();

                await OrderRepository.InsertOneAsync(new Order
                {
                    UserId = message.UserId.ToString(),
                    UserName = message.UserName?.Trim(),
                    DeliveryAddress = message.DeliveryAddress?.Trim(),
                    InvoiceAddress = message.InvoiceAddress?.Trim(),
                    OrderDate = message.OrderDate,
                    Products = products
                });

                Logger.LogInformation($"Order created by UserId:{message.UserId} , UserName:{message.UserName}");
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                throw;
            }
        }
    }
}