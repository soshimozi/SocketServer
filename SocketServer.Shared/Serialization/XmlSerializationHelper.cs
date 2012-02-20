using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using log4net;
using System.Reflection;

namespace SocketServer.Shared.Serialization
{
    public static class XmlSerializationHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static string Serialize<T>(T graph) where T : class
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                StringWriter writer = new StringWriter(sb);
                Serialize<T>(writer, graph);
                writer.Close();
                return sb.ToString();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }

        public static object DeSerialize(string encoded, Type type)
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(type);
                TextReader reader = new StringReader(encoded);
                return serializer.Deserialize(reader);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;

        }
    
        public static void Serialize<T>(TextWriter writer, T graph) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, graph);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
        }

        public static void Serialize<T>(Stream stream, T graph) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stream, graph);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

        }

        public static T DeSerialize<T>(Stream stream) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(stream) as T;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }
        public static T DeSerialize<T>(TextReader reader) where T : class
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(T));
                return serializer.Deserialize(reader) as T;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }


        public static T DeSerialize<T>(string encoded) where T : class
        {
            try
            {
                TextReader reader = new StringReader(encoded);
                return DeSerialize<T>(reader);
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return null;
        }
    }
}
