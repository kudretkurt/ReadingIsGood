using System;
using System.Collections.Generic;
using ReadingIsGood.Shared.CommonTypes;

namespace ReadingIsGood.ProductService.Entities
{
    /// <summary>
    ///     AuditInformation
    /// </summary>
    public class AuditInformation : ValueObject
    {
        /// <summary>
        ///     CreatedBy
        /// </summary>
        public Guid CreatedBy { get; set; }

        /// <summary>
        ///     CreatedDate
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        ///     UpdatedBy
        /// </summary>
        public Guid? UpdatedBy { get; set; }

        /// <summary>
        ///     LastModifiedDate
        /// </summary>
        public DateTime? LastModifiedDate { get; set; }

        /// <summary>
        ///     GetEqualityComponents
        /// </summary>
        /// <returns></returns>
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CreatedBy;
            yield return CreatedDate;
            yield return UpdatedBy;
            yield return LastModifiedDate;
        }
    }
}