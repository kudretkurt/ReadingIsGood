using System;
using NServiceBus;

namespace ReadingIsGood.ProductService.Contracts.Messages
{
    /// <summary>
    ///     UpdateStockInProduct
    /// </summary>
    [Express]
    public class UpdateStockRequest : IMessage
    {
        public UpdateStockRequest(Guid productId, Guid updatedBy, int stock)
        {
            if (productId == Guid.Empty)
                throw new ArgumentNullException($"{nameof(productId)} should not be null or default");

            if (updatedBy == Guid.Empty)
                throw new ArgumentNullException($"{nameof(updatedBy)} should not be null or default");

            Stock = stock;
            ProductId = productId;
            UpdatedById = updatedBy;
        }

        /// <summary>
        ///     Stock (Pozitif yada negatif değer alabilir. Her durumda o anki stok değeri ile girilen değer toplanır.)
        /// </summary>
        public int Stock { get; protected set; }

        /// <summary>
        ///     Product Id
        /// </summary>
        public Guid ProductId { get; protected set; }

        /// <summary>
        ///     UpdatedById
        /// </summary>
        public Guid UpdatedById { get; protected set; }
    }
}