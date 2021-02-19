using System;

namespace ReadingIsGood.Shared.CommonTypes
{
    public class ProductInformationsInOrder
    {
        /// <summary>
        ///     Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Ürün adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Ürün fiyatı
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     Ürün adet bilgisi
        /// </summary>
        public int Count { get; set; }
    }
}