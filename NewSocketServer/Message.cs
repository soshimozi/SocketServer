using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public abstract class Message : IMessage
    {
        /// <summary>
        /// This maps the headers received in a standard http-style message to the value received.
        /// </summary>
        private Dictionary<string, string> headers = new Dictionary<string, string>();

        /// <summary>
        /// This is a single unnamed parameter that is allowed on the command line.
        /// This makes it easy to implement some commands that simply require a flag, i.e. True/False
        /// </summary>
        protected string defaultParameter;
        public string DefaultParameter
        {
            get { return defaultParameter; }
            set { defaultParameter = value; }
        }


        /// <summary>
        /// This stores any binary data received in the payload of the message.
        /// </summary>
        private byte[] data = new byte[0];

        /// <summary>
        /// Returns the name of this message.  By default returns the name of the class.  If the class name ends 
        /// with "Message", it is ommitted.  i.e. SetConfigurationMessage returns "SetConfiguration".
        /// </summary>
        public virtual string Name
        {
            get
            {
                string msgName = GetType().Name;
                if (msgName.EndsWith("Message", StringComparison.CurrentCultureIgnoreCase))
                {
                    msgName = msgName.Substring(0, msgName.Length - "Message".Length);
                }
                return msgName;
            }
        }

        /// <summary>
        /// Returns the header value received in this message.
        /// </summary>
        /// <param name="headerName">The name of the header to query.</param>
        /// <returns>The header value requested.</returns>
        protected virtual string GetHeaderValue(string headerName)
        {
            string headerUppercase = headerName.ToUpper();
            if (!headers.ContainsKey(headerUppercase))
                throw new ProtocolException("Header " + headerUppercase + " not present.");

            return headers[headerUppercase];
        }

        /// <summary>
        /// Returns the header value received in this message, or the provided default value if the value 
        /// has not been set.
        /// </summary>
        /// <param name="headerName">The name of the header to query.</param>
        /// <param name="defaultValue">The value to return if the header does not exist.</param>
        /// <returns>The header value, or the provided default</returns>
        protected virtual T GetHeaderValue<T>(string headerName, T defaultValue) where T : IConvertible
        {
            if (!HeaderExists(headerName))
                return defaultValue;

            string headerUppercase = headerName.ToUpper();
            if (!headers.ContainsKey(headerUppercase))
                throw new ProtocolException(string.Format("Header {0} not present.", headerUppercase));

            if (typeof(T).IsEnum)
                return (T)Enum.Parse(typeof(T), headers[headerUppercase]);

            return (T)Convert.ChangeType(headers[headerUppercase], defaultValue.GetType());
        }

        /// <summary>
        /// Returns an ordered list of header values that begin with the provided prefix.
        /// </summary>
        /// <param name="prefix">The name of the header list.</param>
        /// <returns>An ordered list of strings representing all the value of the requested list.</returns>
        protected List<string> GetHeaderList(string prefix)
        {
            string prefixUppercase = prefix.ToUpper();

            List<string> keyList = headers.Keys
                .Where(key => key.StartsWith(prefixUppercase + "-"))
                .ToList();

            keyList.Sort(String.Compare);

            return keyList.Select(t => headers[t]).ToList();
        }

        /// <summary>
        /// Adds a list of strings to the headers using the given prefix.  On the receiving channel,
        /// the list maintains its order if GetHeaderList is used with the same prefix.
        /// </summary>
        /// <param name="prefix">The name of the list to add</param>
        /// <param name="list">The ordered list of strings to add to the headers as a list.</param>
        protected void AddHeaderList<T>(string prefix, ICollection<T> list)
        {
            if (list == null)
                return;

            IEnumerator<T> iter = list.GetEnumerator();
            int index = 1;
            while (iter.MoveNext())
            {
                string number = index.ToString();
                number = number.PadLeft((int)Math.Log10(list.Count) + 1, '0');

                if (iter.Current == null)
                    continue;

                AddHeader(string.Format("{0}-{1}", prefix, number), iter.Current.ToString());
                index++;
            }
        }

        /// <summary>
        /// Adds a single header to the message with the given name.
        /// </summary>
        /// <param name="headerName">The name of the header to add.</param>
        /// <param name="value">The value of the header to add, as a string.</param>
        public virtual void AddHeader<T>(string headerName, T value)
        {
            if (value == null)
                return;

            string val = value.ToString();
            if (string.IsNullOrEmpty(val)) return;

            string uppercase = headerName.ToUpper();
            if (headers.ContainsKey(uppercase))
                headers.Remove(uppercase);

            headers.Add(uppercase, val);
        }

        /// <summary>
        /// Returns whether a particular header exists in this message.
        /// </summary>
        /// <param name="headerName">The name of the header to query.</param>
        /// <returns>A bool indicating that the header exists in the message.</returns>
        protected virtual bool HeaderExists(string headerName)
        {
            return headers.ContainsKey(headerName.ToUpper());
        }

        /// <summary>
        /// Returns any data received in this message.
        /// </summary>
        /// <returns>The bytes received in the data payload of this message, if any.</returns>
        protected byte[] GetData()
        {
            return data;
        }

        /// <summary>
        /// Sets the binary d to be sent with this message.
        /// </summary>
        /// <param name="d">A binary data payload.</param>
        protected void SetData(byte[] d)
        {
            data = d ?? new byte[0];
        }

        #region IMessage Members

        private string messageID;
        public string MessageID
        {
            get { return messageID; }
            set { messageID = value; }
        }


        /// <summary>
        /// Writes this message out to a stream.  A simple message contains the header/value list and any binary data.
        /// </summary>
        /// <param name="stream">The stream to write to.</param>
        /// <param name="encoding"></param>
        public virtual void Serialize(Stream stream, Encoding encoding)
        {
            lock (headers)
            {
                MapToHeaders();
                WriteHeaders(stream, encoding);
                WriteData(stream);
            }
        }

        /// <summary>
        /// Writes the headers of this message out to the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        private void WriteHeaders(Stream stream, Encoding encoding)
        {
            // Add Content-Length: header if there is data present
            if (data.Length > 0)
                headers["CONTENT-LENGTH"] = data.Length.ToString();

            if (!string.IsNullOrEmpty(messageID))
                headers["MESSAGE-ID"] = messageID;

            StringBuilder headerBuilder = new StringBuilder();
            foreach (KeyValuePair<string, string> pair in headers)
            {
                headerBuilder.Append(pair.Key);
                headerBuilder.Append(":");
                headerBuilder.Append(pair.Value);
                headerBuilder.Append("\r\n");
            }

            // Add final newline to end headers
            headerBuilder.Append("\r\n");

            // Package up into encoding and write to stream
            byte[] headerBytes = encoding.GetBytes(headerBuilder.ToString());
            stream.Write(headerBytes, 0, headerBytes.Length);
        }

        /// <summary>
        /// Writes out any data associated with this message.  Empty data is ingored.
        /// </summary>
        /// <param name="stream"></param>
        private void WriteData(Stream stream)
        {
            // Data payload gets sent out after the header list
            if (data.Length > 0)
                stream.Write(data, 0, data.Length);
        }


        /// <summary>
        /// Reads the message off of the stream.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        public virtual void Deserialize(Stream stream, Encoding encoding)
        {
            lock (headers)
            {
                ReadHeaders(stream, encoding);
                ReadData(stream);
                try { MapFromHeaders(); }
                catch (Exception ex)
                {
                    throw new ProtocolException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Reads the map of header/value pairs into this message dictionary.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        private void ReadHeaders(Stream stream, Encoding encoding)
        {
            headers = ProtocolUtils.GetHeaders(stream, encoding);
            if (headers.ContainsKey("MESSAGE-ID"))
                MessageID = headers["MESSAGE-ID"];
        }

        /// <summary>
        /// Reads any associated data indicated by the Content-Length header.
        /// </summary>
        /// <param name="stream"></param>
        private void ReadData(Stream stream)
        {
            if (headers.ContainsKey("CONTENT-LENGTH"))
            {
                try
                {
                    int length = int.Parse(headers["CONTENT-LENGTH"]);
                    data = new byte[length];
                    int bytesRead = 0;
                    while (bytesRead < length)
                    {
                        bytesRead += stream.Read(data, bytesRead, length - bytesRead);
                    }
                }
                catch (ArgumentNullException ex)
                {
                    throw new ProtocolException(ex.Message);
                }
                catch (OverflowException ex)
                {
                    throw new ProtocolException(ex.Message);
                }
                catch (FormatException ex)
                {
                    throw new ProtocolException(ex.Message);
                }
            }
        }

        /// <summary>
        /// Messages can override this method if there are header requirements that want to be
        /// validated before the message is dispatched.  If the message does not pass validation,
        /// throw a ProtocolException in this method.  
        /// </summary>
        /// <throws>ProtocolException if the message is invalid.</throws>
        public virtual void Validate() { }

        #endregion

        /// <summary>
        /// Override in subclasses to make calls to AddHeader, mapping message fields to headers.
        /// </summary>
        public virtual void MapToHeaders()
        {
        }

        /// <summary>
        /// Override in subclases to make calls to GetHeader, mapping headers into message fields.
        /// </summary>
        public virtual void MapFromHeaders()
        {
        }
    }

}
