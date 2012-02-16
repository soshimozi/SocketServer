using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml.Serialization;

namespace TestServer.Messaging
{
   [XmlInclude(typeof(Encoding))]
    public abstract class Envelope
    {
        protected Envelope()
        {
            MessageEncoding = Encoding.ASCII;
        }

        [XmlIgnore]
        public Encoding MessageEncoding { get; set; }

        [XmlElement("MessageEncoding")]
        public string MessageEncodingString
        {
            get
            {
                if (MessageEncoding is ASCIIEncoding) return "ASCII";
                if (MessageEncoding is UTF8Encoding && MessageEncoding.GetPreamble().Length == 0) return "UTF8";
                if (MessageEncoding is UTF8Encoding && MessageEncoding.GetPreamble().Length == 0) return "UTF8Preamble";
                if (MessageEncoding is UnicodeEncoding) return "Unicode";
                return null;
            }
            set
            {
                switch(value)
                {
                    case "ASCII": MessageEncoding = Encoding.ASCII; break;
                    case "UTF8": MessageEncoding = new UTF8Encoding(false); break;
                    case "UTF8Preamble": MessageEncoding = new UTF8Encoding(true); break;
                    case "Unicode": MessageEncoding = Encoding.Unicode; break;
                }
            }
        }

        public abstract void Serialize(IMessage message, Stream stream);
        public abstract IMessage Deserialize(Stream stream);
    }
}
