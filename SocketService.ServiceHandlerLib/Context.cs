using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace ServiceHandlerLib
{
    public class Context
    {
        private Hashtable _varList = new Hashtable();

        public void Assign(string var, object value)
        {
            _varList.Add(var, value);
        }

        public object getValue(string var)
        {
            if (_varList.ContainsKey(var))
            {
                return _varList[var];
            }
            else
            {
                return null;
            }
        }

    }
}
