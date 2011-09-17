using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SocketService.Framework.SharedObjects
{
    [Serializable]
    public class ServerObject
    {
        private Hashtable data = new Hashtable();

        public ServerObjectRO GetReadOnlyCopy()
        {
            return new ServerObjectRO(data);
        }

        public void SetElementValue(string elementName, object value, ServerObjectDataType dataType)
        {
            if (!data.ContainsKey(elementName))
            {
                data.Add(elementName, new ServerObjectDataHolder());
            }

            (data[elementName] as ServerObjectDataHolder).Value = value;

            (data[elementName] as ServerObjectDataHolder).DataType = dataType;

        }


        public object GetValueForElement(string elementName)
        {
            object value = null;
            if (data.ContainsKey(elementName))
            {
                value = (data[elementName] as ServerObjectDataHolder).Value;
            }

            return value;
        }

        public ServerObjectDataType GetDataTypeForElement(string elementName)
        {
            if( !data.ContainsKey(elementName) )
            {
                throw new ArgumentException();
            }

            return (data[elementName] as ServerObjectDataHolder).DataType;
        }
    }
}
