using FluentValidation;
using ReadingIsGood.API.Models.V1;

namespace ReadingIsGood.API.Validators
{
    /// <summary>
    ///     SaveOrderRequestModelValidators
    /// </summary>
    public class SaveOrderRequestModelValidators : AbstractValidator<SaveOrderRequestModel>
    {
        /// <summary>
        ///     SaveOrderRequestModelValidators
        /// </summary>
        public SaveOrderRequestModelValidators()
        {
            RuleFor(t => t.DeliveryAddress)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(t => t.InvoiceAddress)
                .NotNull()
                .NotEmpty()
                .MaximumLength(200);

            RuleForEach(t => t.Products).SetValidator(new ProductInformationInOrderModelValidators());
        }
    }
}