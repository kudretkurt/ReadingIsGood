using System;

namespace ReadingIsGood.API.Models.V1
{
    /// <summary>
    ///     Sipariş verirken ürüne ait bilgileri içeren object
    /// </summary>
    public class ProductInformationInOrderModel
    {
        /// <summary>
        ///     Ürün id Bilgisi
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Ürün adı
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Ürün Fiyatı
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     Ürün Adedi
        /// </summary>
        public int Count { get; set; }
    }
}