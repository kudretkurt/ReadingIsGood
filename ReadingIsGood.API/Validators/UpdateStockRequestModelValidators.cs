using System;
using FluentValidation;
using ReadingIsGood.API.Models.V1;

namespace ReadingIsGood.API.Validators
{
    /// <summary>
    ///     UpdateStockRequestModelValidators
    /// </summary>
    public class UpdateStockRequestModelValidators : AbstractValidator<UpdateStockRequestModel>
    {
        /// <summary>
        ///     UpdateStockRequestModelValidators
        /// </summary>
        public UpdateStockRequestModelValidators()
        {
            RuleFor(x => x.Stock)
                .NotNull()
                .NotEqual(int.MaxValue)
                .NotEqual(int.MinValue);

            RuleFor(x => x.ProductId)
                .NotNull()
                .NotEqual(Guid.Empty);
        }
    }
}