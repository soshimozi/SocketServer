using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace wox.serial
{
    public interface ISerializeObject
    {
        Object Read(byte [] data);
        byte[] Write(Object obj);
    }
}
