using System.Collections.Generic;

namespace ReadingIsGood.API.Models.V1
{
    /// <summary>
    ///     Sİpariş bilgileri
    /// </summary>
    public class SaveOrderRequestModel
    {
        /// <summary>
        ///     Sİparişin teslim edileceği adres bilgisi
        /// </summary>
        public string DeliveryAddress { get; set; }

        /// <summary>
        ///     Sipariş'e ait faturanın teslim edileceği adres bilgisi
        /// </summary>
        public string InvoiceAddress { get; set; }

        /// <summary>
        ///     Sipariş içindeki ürünlere ait bilgiler
        /// </summary>
        public List<ProductInformationInOrderModel> Products { get; set; }
    }
}