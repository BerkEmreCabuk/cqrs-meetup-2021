using CQRSMeetup.Core.Enums;
using CQRSMeetup.WriteApi.Infrastructures.Database;
using CQRSMeetup.WriteDomain;
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Features.Stock.Commands
{
    public class ChangeStockStatusCommand : IRequest
    {
        public long Id { get; set; }
        public StockStatuses StockStatusId { get; set; }
    }

    public class ChangeStockStatusCommandHandler : AsyncRequestHandler<ChangeStockStatusCommand>
    {
        private readonly IRepository _repository;
        public ChangeStockStatusCommandHandler(IRepository repository)
        {
            _repository = repository;
        }

        protected override async Task Handle(ChangeStockStatusCommand request, CancellationToken cancellationToken)
        {
            var stockEntity = await _repository.GetByIdAsync<StockEntity>(request.Id);
            
            stockEntity.IsNull();
            stockEntity.ChangeStatus(request.StockStatusId);
            
            _repository.Update(stockEntity);
            await _repository.SaveChangesAsync();
        }
    }
}
