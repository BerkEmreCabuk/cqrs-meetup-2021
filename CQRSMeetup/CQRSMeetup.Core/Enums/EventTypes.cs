using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Enums
{
    public enum EventTypes : byte
    {
        UNKNOWN = 0,
        ADDED = 1,
        UPDATED = 2,
        DELETED = 3
    }
}
