using System;
using System.Collections.Generic;
using System.Text;

namespace mqueue_bus
{
    public enum MsgType
    {
        INFO = 0,
        DEBUG,
        INFO_BOX,
        CHANGE_STATE,
        EXCEPTION
    }
}
