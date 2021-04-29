using AutoMapper;
using CQRSMeetup.Core.Enums;
using CQRSMeetup.Core.Mapper;
using CQRSMeetup.WriteApi.Features.Stock.Models;
using CQRSMeetup.WriteApi.Infrastructures.Database;
using CQRSMeetup.WriteDomain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSMeetup.WriteApi.Features.Stock.Commands
{
    public class EntryProductCommand : IMapping, IRequest<BaseResponseModel>
    {
        public long ProductId { get; set; }
        public long LocationId { get; set; }
        public decimal Quantity { get; set; }

        public void CreateMappings(IProfileExpression profileExpression)
        {
            profileExpression.CreateMap<EntryProductCommand, StockEntity>()
                .ForMember(dest => dest.StockStatusId, opt => opt.MapFrom(src => StockStatuses.STANDARD));
        }
    }

    public class EntryProductCommandHandler : IRequestHandler<EntryProductCommand, BaseResponseModel>
    {
        private readonly IRepository _repository;
        private readonly IMapper _mapper;
        public EntryProductCommandHandler(IRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BaseResponseModel> Handle(EntryProductCommand request, CancellationToken cancellationToken)
        {
            var stockEntity = await _repository.FindAsync<StockEntity>(
                x =>
                x.ProductId == request.ProductId &&
                x.LocationId == request.LocationId &&
                x.Status == RecordStatuses.ACTIVE &&
                x.StockStatusId == StockStatuses.STANDARD,
                x => x.Product, x => x.Location);

            if (stockEntity == null)
            {
                stockEntity = _repository.Add(_mapper.Map<StockEntity>(request));
            }
            else
            {
                 stockEntity.EntryProduct(request.Quantity);
                _repository.Update(stockEntity);
            }

            await _repository.SaveChangesAsync();
            return new BaseResponseModel { Id = stockEntity.Id };
        }
    }
}
