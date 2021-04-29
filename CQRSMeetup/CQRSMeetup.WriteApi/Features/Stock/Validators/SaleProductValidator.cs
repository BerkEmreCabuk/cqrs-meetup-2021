using CQRSMeetup.WriteApi.Features.Stock.Commands;
using FluentValidation;

namespace CQRSMeetup.WriteApi.Features.Stock.Validators
{
    public class SaleProductValidator : AbstractValidator<SaleProductCommand>
    {
        public SaleProductValidator()
        {
            RuleFor(x => x).NotEmpty().WithMessage("model not empty");
            RuleFor(x => x.LocationId).GreaterThan(0).WithMessage("LocationId GreaterThan 0(zero)");
            RuleFor(x => x.ProductId).GreaterThan(0).WithMessage("ProductId GreaterThan 0(zero)");
            RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity GreaterThan 0(zero)");
        }
    }
}
