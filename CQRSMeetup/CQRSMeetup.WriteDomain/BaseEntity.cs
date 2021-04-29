using CQRSMeetup.Core.Enums;
using System;

namespace CQRSMeetup.WriteDomain
{
    public class BaseEntity
    {
        public long Id { get; set; }
        public DateTime CreateDate { get; private set; }
        public DateTime? UpdateDate { get; private set; }
        public RecordStatuses Status { get; private set; }
        public virtual EventTypes EventType { get; private set; }

        public void Add()
        {
            CreateDate = DateTime.Now;
            UpdateDate = null;
            Status = RecordStatuses.ACTIVE;
            EventType = EventTypes.ADDED;
        }

        public void Update()
        {
            UpdateDate = DateTime.Now;
            Status = RecordStatuses.ACTIVE;
            EventType = EventTypes.UPDATED;
        }

        public void Delete()
        {
            UpdateDate = DateTime.Now;
            Status = RecordStatuses.PASSIVE;
            EventType = EventTypes.DELETED;
        }
    }
}
