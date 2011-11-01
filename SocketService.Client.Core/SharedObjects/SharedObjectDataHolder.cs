using System;

namespace SocketService.Framework.Client.SharedObjects
{
    [Serializable]
    public class SharedObjectDataHolder
    {
        private int _dataType;
        private object _value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get { return _value; }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public  SharedObjectDataType DataType
        {
            get { return (SharedObjectDataType)_dataType; }
            set { _dataType = (int)value; }
        } 

        private void SetValue(object value)
        {
            _value = value;

            // default to an object
            var dataType = SharedObjectDataType.BzObject;

            if (value == null) return;
            if (value is Int32)
            {
                dataType = SharedObjectDataType.Integer;
            }
            else if (value is Int16)
            {
                dataType = SharedObjectDataType.Short;
            }
            else if (value is Int64)
            {
                dataType = SharedObjectDataType.Long;
            }
            else if (value is String)
            {
                dataType = SharedObjectDataType.String;
            }
            else if (value is float)
            {
                dataType = SharedObjectDataType.Float;
            }
            else if (value is Double)
            {
                dataType = SharedObjectDataType.Double;
            }
            else if (value is Char)
            {
                dataType = SharedObjectDataType.Character;
            }
            else if (value is Byte)
            {
                dataType = SharedObjectDataType.Byte;
            }
            else if (value is Array)
            {
                var valueType = value.GetType();

                if (valueType.GetElementType() == typeof(int))
                {
                    dataType = SharedObjectDataType.IntegerArray;
                }
                else if (valueType.GetElementType() == typeof(Int16))
                {
                    dataType = SharedObjectDataType.ShortArray;
                }
                else if (valueType.GetElementType() == typeof(Int64))
                {
                    dataType = SharedObjectDataType.LongArray;
                }
                else if (valueType.GetElementType() == typeof(string))
                {
                    dataType = SharedObjectDataType.StringArray;
                }
                else if (valueType.GetElementType() == typeof(float))
                {
                    dataType = SharedObjectDataType.FloatArray;
                }
                else if (valueType.GetElementType() == typeof(double))
                {
                    dataType = SharedObjectDataType.DoubleArray;
                }
                else if (valueType.GetElementType() == typeof(char))
                {
                    dataType = SharedObjectDataType.CharacterArray;
                }
                else if (valueType.GetElementType() == typeof(Byte))
                {
                    dataType = SharedObjectDataType.ByteArray;
                }

            }
        
            switch (dataType)
            {
                case SharedObjectDataType.Integer:
                    break;
            }
        }
    }
}
