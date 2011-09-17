using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Client.SharedObjects
{
    [Serializable]
    public class ServerObjectDataHolder
    {
        private int dataType;
        private object value;

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public object Value
        {
            get { return this.value; }
            set { SetValue(value); }
        }

        /// <summary>
        /// Gets or sets the type of the data.
        /// </summary>
        /// <value>
        /// The type of the data.
        /// </value>
        public  ServerObjectDataType DataType
        {
            get { return (ServerObjectDataType)dataType; }
            set { dataType = (int)value; }
        } 

        private void SetValue(object value)
        {
            this.value = value;

            // default to an object
            int dataType = (int)ServerObjectDataType.BzObject;

            if (value != null)
            {
                if (value is Int32)
                {
                    dataType = (int)ServerObjectDataType.Integer;
                }
                else if (value is Int16)
                {
                    dataType = (int)ServerObjectDataType.Short;
                }
                else if (value is Int64)
                {
                    dataType = (int)ServerObjectDataType.Long;
                }
                else if (value is String)
                {
                    dataType = (int)ServerObjectDataType.String;
                }
                else if (value is float)
                {
                    dataType = (int)ServerObjectDataType.Float;
                }
                else if (value is Double)
                {
                    dataType = (int)ServerObjectDataType.Double;
                }
                else if (value is Char)
                {
                    dataType = (int)ServerObjectDataType.Character;
                }
                else if (value is Byte)
                {
                    dataType = (int)ServerObjectDataType.Byte;
                }
                else if (value is Array)
                {
                    Type valueType = value.GetType();

                    if (valueType.GetElementType() == typeof(int))
                    {
                        dataType = (int)ServerObjectDataType.IntegerArray;
                    }
                    else if (value is Int16)
                    {
                        dataType = (int)ServerObjectDataType.ShortArray;
                    }
                    else if (value is Int64)
                    {
                        dataType = (int)ServerObjectDataType.LongArray;
                    }
                    else if (value is String)
                    {
                        dataType = (int)ServerObjectDataType.StringArray;
                    }
                    else if (value is float)
                    {
                        dataType = (int)ServerObjectDataType.FloatArray;
                    }
                    else if (value is Double)
                    {
                        dataType = (int)ServerObjectDataType.DoubleArray;
                    }
                    else if (value is Char)
                    {
                        dataType = (int)ServerObjectDataType.CharacterArray;
                    }
                    else if (value is Byte)
                    {
                        dataType = (int)ServerObjectDataType.ByteArray;
                    }
                }
            }
        }
    }
}
