using CQRSMeetup.Core.Enums;
using System;

namespace CQRSMeetup.ReadDomain
{
    public class StockModel
    {
        public long Id { get; set; }
        public long LocationId { get; set; }
        public string LocationCode { get; set; }
        public string LocationDescription { get; set; }
        public long ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductDescription { get; set; }
        public string ProductSerialNo { get; set; }
        public decimal Quantity { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? UpdateDate { get; set; }
        public RecordStatuses Status { get; set; }
    }
}
