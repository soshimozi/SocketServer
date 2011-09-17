using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SocketService.Framework.SharedObjects;

namespace SocketService.Framework.Data.Domain
{
    public class Room
    {
        public Dictionary<string, ServerObject> _roomVariables = new Dictionary<string, ServerObject>();

        public string Name
        {
            get;
            set;
        }

        public ServerObject GetVariable(string name)
        {
            if (_roomVariables.ContainsKey(name))
            {
                return _roomVariables[name];
            }

            return null;
        }

        public void AddVariable(string name, ServerObject so)
        {
            if (!_roomVariables.ContainsKey(name))
            {
                _roomVariables.Add(name, so);
            }
        }


    }
}
