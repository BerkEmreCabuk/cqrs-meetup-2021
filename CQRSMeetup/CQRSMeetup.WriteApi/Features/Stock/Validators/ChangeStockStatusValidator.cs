using CQRSMeetup.Core.Enums;
using CQRSMeetup.WriteApi.Features.Stock.Commands;
using FluentValidation;

namespace CQRSMeetup.WriteApi.Features.Stock.Validators
{
    public class ChangeStockStatusValidator : AbstractValidator<ChangeStockStatusCommand>
    {
        public ChangeStockStatusValidator()
        {
            RuleFor(x => x).NotEmpty().WithMessage("model not empty");
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id GreaterThan 0(zero)");
            RuleFor(x => x.StockStatusId).NotEqual(StockStatuses.UNKNOWN).WithMessage("Id not equal 0(zero)");
        }
    }
}
