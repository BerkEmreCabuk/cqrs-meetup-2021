using System;
using System.Collections.Generic;
using System.Text;

namespace CQRSMeetup.Core.Enums
{
    public enum StockStatuses : byte
    {
        UNKNOWN = 0,
        STANDARD = 1,
        RETURN = 2,
        BLOCKED = 3
    }
}
