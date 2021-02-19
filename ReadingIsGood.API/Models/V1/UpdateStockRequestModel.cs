using System;

namespace ReadingIsGood.API.Models.V1
{
    /// <summary>
    ///     UpdateStockRequestModel
    /// </summary>
    public class UpdateStockRequestModel
    {
        /// <summary>
        ///     Stock (Pozitif yada negatif değer alabilir. Her durumda o anki stok değeri ile girilen değer toplanır.)
        /// </summary>
        public int Stock { get; set; }

        /// <summary>
        ///     İlgili ürünün id'si
        /// </summary>
        public Guid ProductId { get; set; }
    }
}