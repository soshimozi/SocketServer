using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.Response
{
    [Serializable]
    public class ChangeRoomResponse
    {
        public bool Success
        {
            get;
            set;
        }
    
        public string Room
        {
            get;
            set;
        }
    
    }
}
