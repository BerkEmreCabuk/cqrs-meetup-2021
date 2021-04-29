using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Models
{
    public class RedisConfigModel
    {
        public string PrivateKey { get; set; }
        public string RedisEndPoint { get; set; }
        public int RedisPort { get; set; }
        public int RedisTimeout { get; set; }
    }
}
