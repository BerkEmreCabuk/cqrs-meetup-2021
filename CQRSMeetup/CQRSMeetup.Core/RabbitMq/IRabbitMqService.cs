using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.RabbitMq
{
    public interface IRabbitMqService
    {
        public bool Publish(string channel, object data);
    }
}
