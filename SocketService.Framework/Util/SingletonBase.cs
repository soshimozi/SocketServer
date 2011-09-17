using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SocketService.Framework.Util
{
    public class SingletonBase<T> where T : new()
    {
        private static T _instance = default(T);

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static T Instance
        {
            get
            {
                if( _instance == null )
                {
                    _instance = new T();
                }

                return _instance;
            }
        }
    }
}
