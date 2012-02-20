namespace SocketServer
{
    public class SingletonBase<T> where T : class, new()
    {
        private static T _instance;

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static T Instance
        {
            get { return _instance ?? (_instance = new T()); }
        }
    }
}
