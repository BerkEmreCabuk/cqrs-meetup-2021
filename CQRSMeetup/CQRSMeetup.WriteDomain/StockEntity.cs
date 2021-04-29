using CQRSMeetup.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace CQRSMeetup.WriteDomain
{
    public class StockEntity : BaseEntity
    {
        public long LocationId { get; set; }
        public long ProductId { get; set; }
        public StockStatuses StockStatusId { get; set; }
        public decimal Quantity { get; set; }

        [ForeignKey("ProductId")]
        public ProductEntity Product { get; set; }

        [ForeignKey("LocationId")]
        public LocationEntity Location { get; set; }

        public bool CheckEnoughQuantity(decimal quantity)
        {
            return this.Quantity >= quantity;
        }

        public void SaleProduct(decimal quantity)
        {
            this.Quantity -= quantity;
        }
        public void EntryProduct(decimal quantity)
        {
            this.Quantity += quantity;
        }

        public bool IsValidChangeStatus(StockStatuses stockStatusId)
        {
            return !(this.StockStatusId == StockStatuses.BLOCKED && stockStatusId == StockStatuses.RETURN);
        }

        public void ChangeStatus(StockStatuses stockStatusId)
        {
            if (this.StockStatusId == stockStatusId)
            {
                throw new Exception("Same stock status");
            }
            if (IsValidChangeStatus(stockStatusId))
            {
                throw new Exception("Not change stock status from  blocked status to return status");
            }
            this.StockStatusId = StockStatusId;
        }

        public void IsNull()
        {
            if (this == null)
            {
                throw new Exception("Not found stock");
            }
        }
    }
}
