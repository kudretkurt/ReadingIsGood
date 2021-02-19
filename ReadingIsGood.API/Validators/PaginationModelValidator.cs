using FluentValidation;
using ReadingIsGood.API.Models;

namespace ReadingIsGood.API.Validators
{
    /// <summary>
    /// </summary>
    public class PaginationModelValidator : AbstractValidator<PaginationModel>
    {
        /// <summary>
        /// </summary>
        public PaginationModelValidator()
        {
            RuleFor(t => t.PageNumber)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .LessThan(int.MaxValue);

            RuleFor(t => t.PageSize)
                .NotNull()
                .NotEmpty()
                .GreaterThan(0)
                .LessThanOrEqualTo(10);
        }
    }
}