using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Data
{
    public partial class Room
    {
        public Room()
        {
            Id = AutoIdElement.GetNextID();
        }
    }
}
