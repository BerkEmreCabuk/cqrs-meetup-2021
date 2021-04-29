using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Enums
{
    public enum RecordStatuses : byte
    {
        UNKNOWN = 0,
        ACTIVE = 1,
        PASSIVE = 2
    }
}
