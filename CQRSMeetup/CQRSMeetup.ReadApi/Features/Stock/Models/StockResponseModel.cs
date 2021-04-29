using AutoMapper;
using CQRSMeetup.Core.Enums;
using CQRSMeetup.Core.Mapper;
using CQRSMeetup.ReadDomain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CQRSMeetup.ReadApi.Features.Stock.Models
{
    public class StockResponseModel : IMapping
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        public string LocationCode { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public RecordStatuses Status { get; set; }

        public void CreateMappings(IProfileExpression profileExpression)
        {
            profileExpression.CreateMap<StockModel, StockResponseModel>();
        }
    }
}
