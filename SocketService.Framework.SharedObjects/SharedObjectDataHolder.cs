using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.SharedObjects
{
    [Serializable]
    public class SharedObjectDataHolder
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
        public  SharedObjectDataType DataType
        {
            get { return (SharedObjectDataType)dataType; }
            set { dataType = (int)value; }
        } 

        private void SetValue(object value)
        {
            this.value = value;

            // default to an object
            int dataType = (int)SharedObjectDataType.BzObject;

            if (value != null)
            {
                if (value is Int32)
                {
                    dataType = (int)SharedObjectDataType.Integer;
                }
                else if (value is Int16)
                {
                    dataType = (int)SharedObjectDataType.Short;
                }
                else if (value is Int64)
                {
                    dataType = (int)SharedObjectDataType.Long;
                }
                else if (value is String)
                {
                    dataType = (int)SharedObjectDataType.String;
                }
                else if (value is float)
                {
                    dataType = (int)SharedObjectDataType.Float;
                }
                else if (value is Double)
                {
                    dataType = (int)SharedObjectDataType.Double;
                }
                else if (value is Char)
                {
                    dataType = (int)SharedObjectDataType.Character;
                }
                else if (value is Byte)
                {
                    dataType = (int)SharedObjectDataType.Byte;
                }
                else if (value is Array)
                {
                    Type valueType = value.GetType();

                    if (valueType.GetElementType() == typeof(int))
                    {
                        dataType = (int)SharedObjectDataType.IntegerArray;
                    }
                    else if (value is Int16)
                    {
                        dataType = (int)SharedObjectDataType.ShortArray;
                    }
                    else if (value is Int64)
                    {
                        dataType = (int)SharedObjectDataType.LongArray;
                    }
                    else if (value is String)
                    {
                        dataType = (int)SharedObjectDataType.StringArray;
                    }
                    else if (value is float)
                    {
                        dataType = (int)SharedObjectDataType.FloatArray;
                    }
                    else if (value is Double)
                    {
                        dataType = (int)SharedObjectDataType.DoubleArray;
                    }
                    else if (value is Char)
                    {
                        dataType = (int)SharedObjectDataType.CharacterArray;
                    }
                    else if (value is Byte)
                    {
                        dataType = (int)SharedObjectDataType.ByteArray;
                    }
                }
            }
        }
    }
}
