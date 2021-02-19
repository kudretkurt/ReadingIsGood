using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NServiceBus;
using ReadingIsGood.OrderService.Contracts.Commands;
using ReadingIsGood.ProductService.Contracts.Commands;
using ReadingIsGood.ProductService.Repositories;
using ReadingIsGood.Shared;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.ProductService.Handlers.CommandHandlers
{
    public class ProductHandler : IHandleMessages<OrderControlCommand>
    {
        public IProductRepository ProductRepository { get; set; }
        public ILogger Logger { get; set; }

        public async Task Handle(OrderControlCommand message, IMessageHandlerContext context)
        {
            var listOfTuple = message.Products.Select(product => Tuple.Create(product.Id, -product.Count)).ToList();

            var result = await ProductRepository.UpdateStocks(listOfTuple);

            if (result == listOfTuple.Count)
            {
                Logger.LogInformation($"Stock successfully controlled . UserId:{message.UserId}");

                var sendOptions = new SendOptions();
                sendOptions.SetDestination(
                    ApplicationConfiguration.Instance.GetValue<string>("OrderService:EndpointName"));
                sendOptions.RequireImmediateDispatch();

                var tokenSource = new CancellationTokenSource();
                tokenSource.CancelAfter(TimeSpan.FromSeconds(5));

                var products = message.Products.Select(t => new ProductInformationsInOrder
                {
                    Id = t.Id,
                    Count = t.Count,
                    Name = t.Name,
                    Price = t.Price
                }).ToList();

                await context.Send(
                    new SaveOrderCommand(message.UserId, message.UserName, message.DeliveryAddress,
                        message.InvoiceAddress, products), sendOptions);
                return;
            }

            //TODO:Kudret-->Eğer sipariş içindeki ürünlere ait yapılan stok kontrolleri başarılı olmadı ise gerekli notificationMessage ları publish edilir.
            //(Siparişiniz .... sorunlarından ötürü gerçekleştirilemiyor vs gibi)
            //Ben şimdilik basit bir log atıyorum.

            Logger.LogError(
                $"Sipariş ile ilgili problem oluşmuştur. UserId:{message.UserId}");
        }
    }
}