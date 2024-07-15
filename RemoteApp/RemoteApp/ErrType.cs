using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RemoteApp
{
    public enum ErrType
    {
        SUCCESS = 0,
        GET_RAPP_ERR = 1,
        RAPP_EXIST = 2,
        RAPP_NOT_EXIST = 3,
        CAN_NOT_UNINSTALL = 4,
        RAPP_NOT_IN_PATH = 5,
        CNOT_SEND_RAPP = 6,
    }


}
