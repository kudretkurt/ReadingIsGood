using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.OrderService.Contracts.Messages;
using ReadingIsGood.OrderService.Entities;
using ReadingIsGood.OrderService.Repositories;
using ReadingIsGood.Shared;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.OrderService.Handlers.CallbackHandlers
{
    public class OrderCallbackHandler : IHandleMessages<OrderViewRequest>
    {
        public ILogger Logger { get; set; }
        public IMongoRepository<Order> OrderRepository { get; set; }

        public async Task Handle(OrderViewRequest message, IMessageHandlerContext context)
        {
            try
            {
                var result = await OrderRepository.FilterBy(t => t.UserId == message.UserId.ToString(),
                    message.PageNumber, message.PageSize);
                var orderCallbackResults = result.ToList().Select(t => new OrderCallbackResult()
                {
                    DeliveryAddress = t.DeliveryAddress,
                    InvoiceAddress = t.InvoiceAddress,
                    OrderDate = t.OrderDate,
                    UserId = t.UserId,
                    UserName = t.UserName,
                    Products = t.Products.Select(a => new ProductInformationsInOrder()
                    {
                        Count = a.Count,
                        Name = a.Name,
                        Price = a.Price
                    }).ToList()
                }).ToList();

                await context.Reply(CallbackResult<List<OrderCallbackResult>>.SuccessResult(orderCallbackResults));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "");
                await context.Reply(CallbackResult<List<OrderCallbackResult>>.ErrorResult(null,
                    "Bilinmeyen Bir Hatadan Dolayı Siparişlerinizi Görüntüleyemiyoruz"));
            }
        }
    }
}