using System;
using System.Collections.Generic;
using ReadingIsGood.OrderService.Attributes;

namespace ReadingIsGood.OrderService.Entities
{
    [BsonCollection("orders")]
    public class Order : Document
    {
        /// <summary>
        ///     Sipariş veren kullanıcı/müşteri Id bilgisi
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Sipariş veren kullanıcı adı bilgisi
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     Sipariş teslim adresi
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        ///     Sipariş fatura Adresi
        /// </summary>
        public string InvoiceAddress { get; set; }

        /// <summary>
        ///     Sipariş tarihi
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        ///     Sipariş içindeki ürünlerin bilgisi
        /// </summary>
        public List<Product> Products { get; set; }
    }
}