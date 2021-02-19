using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ReadingIsGood.ProductService.Entities;

namespace ReadingIsGood.ProductService.Repositories
{
    /// <summary>
    ///     IProductRepository
    /// </summary>
    public interface IProductRepository
    {
        /// <summary>
        ///     Girilen stock değeri ile güncel stok değeri toplanır,update edilir.
        /// </summary>
        /// <param name="productId"></param>
        /// <param name="stockValue"></param>
        /// <returns></returns>
        Task<int> UpdateStock(Guid productId, int stockValue);


        /// <summary>
        ///     Girilen stock değeri ile güncel stok değeri toplanır,update edilir.
        /// </summary>
        /// <param name="productStockValues"></param>
        /// <returns></returns>
        Task<int> UpdateStocks(List<Tuple<Guid, int>> productStockValues);

        /// <summary>
        ///     GetProductById
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        Task<Product> GetProductById(Guid productId);

        /// <summary>
        ///     SaveProduct
        /// </summary>
        /// <param name="product"></param>
        /// <returns></returns>
        Task<int> SaveProduct(Product product);
    }
}