using System;
using System.IO;
using NServiceBus;

namespace ReadingIsGood.ProductService.Contracts.Messages
{
    /// <summary>
    ///     SaveProductRequest
    /// </summary>
    [Express]
    public class SaveProductRequest : IMessage
    {
        public SaveProductRequest(string productName, int stock, decimal price, Guid createdBy)
        {
            if (string.IsNullOrEmpty(productName?.Trim())) throw new ArgumentNullException(nameof(productName));

            if (stock <= 0) throw new InvalidDataException(nameof(stock));

            if (createdBy == Guid.Empty) throw new ArgumentNullException(nameof(createdBy));

            ProductName = productName?.Trim();
            Stock = stock;
            Price = price;
            CreatedUserId = createdBy;
        }

        /// <summary>
        ///     Ürün Adı
        /// </summary>
        public string ProductName { get; protected set; }

        /// <summary>
        ///     Ürün Stok Miktarı
        /// </summary>
        public int Stock { get; protected set; }

        /// <summary>
        ///     Ürün Fiyatı
        /// </summary>
        public decimal Price { get; protected set; }

        /// <summary>
        ///     İlgili requesti yapan user Bilgisi
        /// </summary>
        public Guid CreatedUserId { get; protected set; }
    }
}