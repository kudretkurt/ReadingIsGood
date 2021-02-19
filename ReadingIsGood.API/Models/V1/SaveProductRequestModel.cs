namespace ReadingIsGood.API.Models.V1
{
    /// <summary>
    ///     SaveProductRequestModel
    /// </summary>
    public class SaveProductRequestModel
    {
        /// <summary>
        ///     Ürün ismi
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Ürün ücreti
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     Ürün stok bilgisi
        /// </summary>
        public int Stock { get; set; }
    }
}