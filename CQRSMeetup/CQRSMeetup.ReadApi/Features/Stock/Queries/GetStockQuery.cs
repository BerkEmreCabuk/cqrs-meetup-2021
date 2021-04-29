using AutoMapper;
using CQRSMeetup.Core.Redis;
using CQRSMeetup.ReadApi.Features.Stock.Models;
using CQRSMeetup.ReadDomain;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CQRSMeetup.ReadApi.Features.Stock.Queries
{
    public class GetStockQuery : IRequest<StockResponseModel>
    {
        public GetStockQuery(long id)
        {
            this.Id = id;
        }
        public long Id { get; set; }
    }

    public class GetStockQueryHandler : IRequestHandler<GetStockQuery, StockResponseModel>
    {
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public GetStockQueryHandler(IMapper mapper, IRedisService redisService)
        {
            _mapper = mapper;
            _redisService = redisService;
        }
        public async Task<StockResponseModel> Handle(GetStockQuery request, CancellationToken cancellationToken)
        {
            var responseModel = new StockResponseModel();
            var stockModel = _redisService.Get<StockModel>($"stock:{request.Id}");
            if (stockModel != null)
            {
                responseModel = _mapper.Map<StockResponseModel>(stockModel);
            }
            return responseModel;
        }
    }
}
