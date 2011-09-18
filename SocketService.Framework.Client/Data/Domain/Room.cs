using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Client.Data.Domain
{
    public class Room
    {
        private Dictionary<string, ServerObject> _roomVariables = new Dictionary<string, ServerObject>();

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public ServerObject GetVariable(string name)
        {
            if (_roomVariables.ContainsKey(name))
            {
                return _roomVariables[name];
            }

            return null;
        }

        /// <summary>
        /// Adds the variable.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="so">The so.</param>
        public void AddVariable(string name, ServerObject so)
        {
            if (!_roomVariables.ContainsKey(name))
            {
                _roomVariables.Add(name, so);
            }
        }


    }
}
