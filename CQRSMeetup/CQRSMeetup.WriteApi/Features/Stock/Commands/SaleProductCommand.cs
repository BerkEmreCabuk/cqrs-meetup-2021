using CQRSMeetup.Core.Enums;
using CQRSMeetup.WriteApi.Infrastructures.Database;
using CQRSMeetup.WriteDomain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Features.Stock.Commands
{
    public class SaleProductCommand : IRequest
    {
        public long ProductId { get; set; }
        public long LocationId { get; set; }
        public decimal Quantity { get; set; }
    }

    public class SaleProductCommandHandler : AsyncRequestHandler<SaleProductCommand>
    {
        private readonly IRepository _repository;
        public SaleProductCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override async Task Handle(SaleProductCommand request, CancellationToken cancellationToken)
        {
            var stockEntity = await _repository.FindAsync<StockEntity>(
                x =>
                x.ProductId == request.ProductId &&
                x.LocationId == request.LocationId &&
                x.Status == RecordStatuses.ACTIVE &&
                x.StockStatusId == StockStatuses.STANDARD);

            stockEntity.IsNull();
            if (stockEntity.CheckEnoughQuantity(request.Quantity))
            {
                throw new Exception("not enough quantity");
            }

            stockEntity.SaleProduct(request.Quantity);
            _repository.Update(stockEntity);
            await _repository.SaveChangesAsync();
        }
    }
}
