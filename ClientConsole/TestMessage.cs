using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer;
using ProtoBuf;

namespace ClientConsole
{
    [ProtoContract]
    class TestMessage 
    {
        public TestMessage()
        {

        }

        [ProtoMember(1, IsRequired = true)]
        public string Name
        {
            get;
            set;
        }



    }
}
