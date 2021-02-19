using System;
using FluentValidation;
using ReadingIsGood.API.Models.V1;

namespace ReadingIsGood.API.Validators
{
    /// <summary>
    ///     ProductInformationInOrderModelValidators
    /// </summary>
    public class ProductInformationInOrderModelValidators : AbstractValidator<ProductInformationInOrderModel>
    {
        /// <summary>
        ///     ProductInformationInOrderModelValidators
        /// </summary>
        public ProductInformationInOrderModelValidators()
        {
            RuleFor(t => t.Count)
                .NotNull()
                .NotEqual(int.MaxValue)
                .GreaterThan(0);

            RuleFor(t => t.Name)
                .NotNull()
                .MaximumLength(100)
                .MinimumLength(3);

            RuleFor(t => t.Id)
                .NotNull()
                .NotEmpty()
                .NotEqual(Guid.Empty);

            RuleFor(t => t.Price)
                .NotNull()
                .GreaterThanOrEqualTo(0);
        }
    }
}