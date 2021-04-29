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
    public class GetStocksQuery : IRequest<List<StockResponseModel>>
    {
    }

    public class GetStocksQueryHandler : IRequestHandler<GetStocksQuery, List<StockResponseModel>>
    {
        private readonly IMapper _mapper;
        private readonly IRedisService _redisService;

        public GetStocksQueryHandler(IMapper mapper, IRedisService redisService)
        {
            _mapper = mapper;
            _redisService = redisService;
        }
        public async Task<List<StockResponseModel>> Handle(GetStocksQuery request, CancellationToken cancellationToken)
        {
            var responseModel = new List<StockResponseModel>();
            var stockModel = _redisService.GetAll<StockModel>($"stock");
            if (stockModel.Any())
            {
                responseModel = _mapper.Map<List<StockResponseModel>>(stockModel.ToList());
            }
            return responseModel;
        }
    }
}
