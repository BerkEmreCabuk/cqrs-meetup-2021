using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Models
{
    public class RabbitMqConfigModel
    {
        public string RabbitMqHostname { get; set; }
        public string RabbitMqUsername { get; set; }
        public string RabbitMqPassword { get; set; }
    }
}
