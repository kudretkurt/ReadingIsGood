namespace ReadingIsGood.OrderService.Entities
{
    public class Product
    {
        /// <summary>
        ///     Ürün Id
        /// </summary>
        public string Id { get; set; }

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