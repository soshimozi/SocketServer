using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SocketService.Framework.SharedObjects
{
 
    [Serializable]
    public class ServerObjectRO
    {
        ServerObject innerObject;
        private Hashtable data;

        public ServerObjectRO(string name, object value, ServerObjectDataType dataType)
        {
            innerObject = new ServerObject();
            innerObject.SetElementValue(name, value, dataType);
        }

        public ServerObjectRO(Hashtable data)
        {
            this.data = data;
        }

        public object GetValueForElement(string name)
        {
            return innerObject.GetValueForElement(name);
        }
    }
}
