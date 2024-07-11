using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace cnsoftbei_A8
{
    public enum ErrType
    {
        SUCCESS = 0,
        USER_INFO_ERR = 3,
        GET_RAPP_ERR = 4,
        CONNECT_COUNT_OUT = 5,
        RAPP_EXIST = 6,
        RAPP_NOT_EXIST = 7,
        LOCK_USER_FAILED = 8,
        RDP_NOT_OPEN = 9,
    }
}
