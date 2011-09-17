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

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerObjectRO"/> class.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="dataType">Type of the data.</param>
        public ServerObjectRO(string name, object value, ServerObjectDataType dataType)
        {
            innerObject = new ServerObject();
            innerObject.SetElementValue(name, value, dataType);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerObjectRO"/> class.
        /// </summary>
        /// <param name="data">The data.</param>
        public ServerObjectRO(Hashtable data)
        {
            this.data = data;
        }

        /// <summary>
        /// Gets the value for element.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public object GetValueForElement(string name)
        {
            return innerObject.GetValueForElement(name);
        }
    }
}
