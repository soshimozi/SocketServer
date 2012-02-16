using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TestServer.Messaging;

namespace TestServer
{
    public class TestMessageTwo : IMessage
    {
        public string MessageID
        {
            get; set;
        }

        public void Validate()
        {
        }

        public string Name
        {
            get { return "TestMessageTwo"; }
        }


        public void Deserialize(System.IO.Stream stream, Encoding encoding)
        {
            throw new NotImplementedException();
        }

        public void Serialize(System.IO.Stream stream, Encoding encoding)
        {
            throw new NotImplementedException();
        }
    }
}
