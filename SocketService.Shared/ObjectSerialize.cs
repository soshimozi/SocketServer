using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace SocketServer.Shared
{
    public static class ObjectSerialize
    {
        /// <summary>
        /// Serializes the specified graph.
        /// </summary>
        /// <param name="graph">The graph.</param>
        /// <returns></returns>
        public static byte[] Serialize(object graph)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, graph);

                return ms.ToArray();
            }
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static object Deserialize(byte[] data)
        {
            using (var ms = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms);
            }
        }

        /// <summary>
        /// Deserializes the specified data.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static T Deserialize<T>(byte[] data) where T : class
        {
            using (var ms = new MemoryStream(data))
            {
                var formatter = new BinaryFormatter();
                return formatter.Deserialize(ms) as T;
            }
        }
    }
}