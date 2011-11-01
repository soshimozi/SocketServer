using System;

namespace SocketService.Framework.Client.SharedObjects
{
 
    [Serializable]
    public class SharedObjectRO
    {
        readonly SharedObject _innerObject;

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedObjectRO"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public SharedObjectRO(string name, object value, SharedObjectDataType dataType)
        {
            _innerObject = new SharedObject();
            _innerObject.SetElementValue(name, value, dataType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SharedObjectRO"/> class.
        /// </summary>
        public SharedObjectRO()
        {
        }

        /// <summary>
        /// Gets the value for element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object GetValueForElement(string name)
        {
            return _innerObject.GetValueForElement(name);
        }
    }
}
