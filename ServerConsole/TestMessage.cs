using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketServer;

namespace ServerConsole
{
    class TestMessage : Message
    {
        public TestMessage()
        {

        }

        public string Message
        {
            get { return GetHeaderValue("MESSAGE"); }
            set { AddHeader("MESSAGE", value); }
        }

        #region IMessage Members


        public override string Name
        {
            get { return "TEST"; }
        }

        #endregion


    }
}
