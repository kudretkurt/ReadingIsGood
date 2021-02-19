using System;
using System.ComponentModel.DataAnnotations;

namespace ReadingIsGood.ProductService.Entities
{
    public class Product
    {
        /// <summary>
        ///     Id
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Price
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        ///     Stock
        /// </summary>
        [ConcurrencyCheck]
        public int Stock { get; set; }

        /// <summary>
        ///     AuditInformation
        /// </summary>
        public AuditInformation AuditInformation { get; set; }
    }
}