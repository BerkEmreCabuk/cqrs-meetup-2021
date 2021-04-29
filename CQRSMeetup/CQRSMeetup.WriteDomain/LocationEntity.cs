using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.WriteDomain
{
    public class LocationEntity : BaseEntity
    {
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
