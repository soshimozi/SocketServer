﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService
{
    public interface IServerContext
    {
        List<ClientConnection> ConnectionList { get; }
    }
}
