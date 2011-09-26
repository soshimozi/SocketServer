using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace SocketService.Framework.SharedObjects
{
    [Serializable]
    public class SharedObject
    {
        private Hashtable data = new Hashtable();

        public SharedObject()
        {
        }

        public SharedObject(object value)
        {
            SharedObjectDataType dt = DataTypeFromType(value.GetType());
            SetElementValue("", value, dt);
        }

        private SharedObjectDataType DataTypeFromType(Type t)
        {
            if (t.IsAssignableFrom(typeof(string)))
            {
                return SharedObjectDataType.String;
            }
            else if (t.IsAssignableFrom(typeof(int)))
            {
                return SharedObjectDataType.Integer;
            }
            else if(t.IsAssignableFrom(typeof(long)))
            {
                return SharedObjectDataType.Long;
            }
            else if (t.IsAssignableFrom(typeof(double)))
            {
                return SharedObjectDataType.Double;
            }
            else if(t.IsAssignableFrom(typeof(byte)))
            {
                return SharedObjectDataType.Byte;
            }
            else if (t.IsAssignableFrom(typeof(char)))
            {
                return SharedObjectDataType.Character;
            }
            else if (t.IsClass)
            {
                return SharedObjectDataType.BzObject;
            }
            else if( t.IsArray)
            {
                // check array type
                return DataTypeFromType(t.GetElementType());
            }
            else
            {
                return SharedObjectDataType.BzObject;
            }


        }

        /// <summary>
        /// Gets the read only copy.
        /// </summary>
        /// <returns></returns>
        public SharedObjectRO GetReadOnlyCopy()
        {
            return new SharedObjectRO(data);
        }

        /// <summary>
        /// Sets the element value.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public void SetElementValue(string elementName, object value, SharedObjectDataType dataType)
        {
            if (!data.ContainsKey(elementName))
            {
                data.Add(elementName, new SharedObjectDataHolder());
            }

            (data[elementName] as SharedObjectDataHolder).Value = value;

            (data[elementName] as SharedObjectDataHolder).DataType = dataType;

        }


        /// <summary>
        /// Gets the value for element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public object GetValueForElement(string elementName)
        {
            object value = null;
            if (data.ContainsKey(elementName))
            {
                value = (data[elementName] as SharedObjectDataHolder).Value;
            }

            return value;
        }

        /// <summary>
        /// Gets the data type for element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public SharedObjectDataType GetDataTypeForElement(string elementName)
        {
            if( !data.ContainsKey(elementName) )
            {
                throw new ArgumentException();
            }

            return (data[elementName] as SharedObjectDataHolder).DataType;
        }
    }
}
