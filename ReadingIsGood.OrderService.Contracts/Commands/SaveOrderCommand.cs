using System;
using System.Collections.Generic;
using NServiceBus;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.OrderService.Contracts.Commands
{
    public class SaveOrderCommand : ICommand
    {
        /// <summary>
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="deliveryAddress"></param>
        /// <param name="invoiceAddress"></param>
        /// <param name="products"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SaveOrderCommand(Guid userId, string userName, string deliveryAddress, string invoiceAddress,
            List<ProductInformationsInOrder> products)
        {
            if (userId == Guid.Empty) throw new ArgumentNullException(nameof(userId));

            if (string.IsNullOrEmpty(userName?.Trim())) throw new ArgumentNullException(nameof(userName));

            if (string.IsNullOrEmpty(deliveryAddress?.Trim())) throw new ArgumentNullException(nameof(deliveryAddress));

            if (string.IsNullOrEmpty(invoiceAddress?.Trim())) throw new ArgumentNullException();

            if (products == null || products?.Count == 0) throw new ArgumentNullException(nameof(products));

            UserId = userId;
            UserName = userName?.Trim();
            DeliveryAddress = deliveryAddress?.Trim();
            InvoiceAddress = invoiceAddress?.Trim();
            OrderDate = DateTime.UtcNow;
            Products = products;
        }

        /// <summary>
        ///     UserId
        /// </summary>
        public Guid UserId { get; protected set; }

        /// <summary>
        ///     UserName
        /// </summary>
        public string UserName { get; protected set; }

        /// <summary>
        ///     OrderDate
        /// </summary>
        public DateTime OrderDate { get; protected set; }

        /// <summary>
        ///     DeliveryAddress
        /// </summary>
        public string DeliveryAddress { get; protected set; }

        /// <summary>
        ///     InvoiceAddress
        /// </summary>
        public string InvoiceAddress { get; protected set; }

        /// <summary>
        ///     Products
        /// </summary>
        public IEnumerable<ProductInformationsInOrder> Products { get; protected set; }
    }
}