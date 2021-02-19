using FluentValidation;
using ReadingIsGood.API.Models.V1;

namespace ReadingIsGood.API.Validators
{
    /// <summary>
    ///     SaveProductRequestModelValidators
    /// </summary>
    public class SaveProductRequestModelValidators : AbstractValidator<SaveProductRequestModel>
    {
        /// <summary>
        ///     SaveProductRequestModelValidators(ctor)
        /// </summary>
        public SaveProductRequestModelValidators()
        {
            RuleFor(x => x.Name)
                .NotNull()
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(x => x.Stock)
                .NotNull()
                .NotEqual(int.MaxValue)
                .GreaterThan(0);

            RuleFor(x => x.Price)
                .NotNull()
                .GreaterThanOrEqualTo(0);
        }
    }
}