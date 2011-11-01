using System;
using System.Collections;

namespace SocketService.Framework.Client.SharedObjects
{
    [Serializable]
    public class SharedObject
    {
        private readonly Hashtable _data = new Hashtable();

        public SharedObject()
        {
        }

        public SharedObject(object value)
        {
            var dt = DataTypeFromType(value.GetType());
            SetElementValue("", value, dt);
        }

        private static SharedObjectDataType DataTypeFromType(Type t)
        {
            if (t.IsAssignableFrom(typeof(string)))
            {
                return SharedObjectDataType.String;
            }
            
            if (t.IsAssignableFrom(typeof(int)))
            {
                return SharedObjectDataType.Integer;
            }
            
            if(t.IsAssignableFrom(typeof(long)))
            {
                return SharedObjectDataType.Long;
            }

            if (t.IsAssignableFrom(typeof(double)))
            {
                return SharedObjectDataType.Double;
            }

            if(t.IsAssignableFrom(typeof(byte)))
            {
                return SharedObjectDataType.Byte;
            }

            if (t.IsAssignableFrom(typeof(char)))
            {
                return SharedObjectDataType.Character;
            }

            if (t.IsClass)
            {
                return SharedObjectDataType.BzObject;
            }

            if( t.IsArray)
            {
                // check array type
                return DataTypeFromType(t.GetElementType());
            }

            return SharedObjectDataType.BzObject;
        }

        /// <summary>
        /// Gets the read only copy.
        /// </summary>
        /// <returns></returns>
        public SharedObjectRO GetReadOnlyCopy()
        {
            return new SharedObjectRO();
        }

        /// <summary>
        /// Sets the element value.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public void SetElementValue(string elementName, object value, SharedObjectDataType dataType)
        {
            if (!_data.ContainsKey(elementName))
            {
                _data.Add(elementName, new SharedObjectDataHolder());
            }

            var sharedObjectDataHolder = _data[elementName] as SharedObjectDataHolder;
            if (sharedObjectDataHolder != null)
                sharedObjectDataHolder.Value = value;

            var objectDataHolder = _data[elementName] as SharedObjectDataHolder;
            if (objectDataHolder != null)
                objectDataHolder.DataType = dataType;
        }


        /// <summary>
        /// Gets the value for element.
        /// </summary>
        /// <param name="elementName">Name of the element.</param>
        /// <returns></returns>
        public object GetValueForElement(string elementName)
        {
            object value = null;
            if (_data.ContainsKey(elementName))
            {
                var sharedObjectDataHolder = _data[elementName] as SharedObjectDataHolder;
                if (sharedObjectDataHolder != null)
                    value = sharedObjectDataHolder.Value;
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
            if( !_data.ContainsKey(elementName) )
            {
                throw new ArgumentException();
            }

            var sharedObjectDataHolder = _data[elementName] as SharedObjectDataHolder;
            if (sharedObjectDataHolder != null)
                return sharedObjectDataHolder.DataType;
            
            
            return SharedObjectDataType.None;
            
        }
    }
}
