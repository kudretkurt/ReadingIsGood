using System;
using System.Collections.Generic;
using NServiceBus;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.OrderService.Contracts.Messages
{
    [Express]
    public class OrderCallbackResult : IMessage
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string DeliveryAddress { get; set; }
        public string InvoiceAddress { get; set; }
        public DateTime OrderDate { get; set; }
        public List<ProductInformationsInOrder> Products { get; set; }
    }
}