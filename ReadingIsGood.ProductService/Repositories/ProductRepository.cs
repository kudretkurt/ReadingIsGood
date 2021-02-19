using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ReadingIsGood.ProductService.Context;
using ReadingIsGood.ProductService.Entities;
using ReadingIsGood.Shared.Exceptions;

namespace ReadingIsGood.ProductService.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ProductServiceDbContext _productServiceDbContext;

        public ProductRepository(ProductServiceDbContext productServiceDbContext)
        {
            _productServiceDbContext = productServiceDbContext;
        }


        public async Task<int> UpdateStock(Guid productId, int stockValue)
        {
            try
            {
                var entity = await _productServiceDbContext.Products.FindAsync(productId);

                if (entity == null) throw new ArgumentNullException($"ProductId: {productId} does not exist");

                entity.Stock += stockValue;

                if (entity.Stock < 0)
                    throw new InvalidOperationException($"Stock should not be negative . ProductId:{productId}");

                return await _productServiceDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }
        }

        public async Task<int> UpdateStocks(List<Tuple<Guid, int>> productStockValues)
        {
            try
            {
                foreach (var productStock in productStockValues)
                {
                    var entity = await _productServiceDbContext.Products.FindAsync(productStock.Item1);

                    if (entity == null)
                        throw new ArgumentNullException($"ProductId: {productStock.Item1} does not exist");

                    entity.Stock += productStock.Item2;

                    if (entity.Stock < 0)
                        throw new InvalidOperationException(
                            $"Stock should not be negative . ProductId:{productStock.Item1}");
                }

                return await _productServiceDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw new OptimisticConcurrencyException();
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public async Task<Product> GetProductById(Guid productId)
        {
            var product = await _productServiceDbContext.Products.AsNoTracking().FirstOrDefaultAsync();

            if (product == null) throw new ArgumentNullException($"ProductId:{productId} does not exist.");

            return product;
        }

        public async Task<int> SaveProduct(Product product)
        {
            if (product == null) throw new ArgumentNullException($"{nameof(product)} can not be null");

            _productServiceDbContext.Add(product);
            return await _productServiceDbContext.SaveChangesAsync();
        }
    }
}