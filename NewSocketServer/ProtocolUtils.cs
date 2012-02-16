using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace SocketServer
{
    public static class ProtocolUtils
    {
        private static readonly Regex headerPattern = new Regex("(.+?):(.*)");

        // Retreives all header lines up to the \r\n\r\n, and returns an
        // array container each header line, without the \r\n.
        public static Dictionary<string, string> GetHeaders(Stream stream, Encoding encoding)
        {
            string[] headerLines = GetHeaderLines(stream, encoding);
            return ParseHeaders(headerLines);
        }

        public static String[] GetHeaderLines(Stream stream, Encoding encoding)
        {
            List<string> headers = new List<string>();
            while (true)
            {
                String header;
                try
                {
                    header = ReadLine(stream, encoding);
                }
                catch (Exception)
                {
                    break;
                }

                if (header.Length == 0)
                {
                    // Empty Line.  End of headers
                    break;
                }
                // Else we add the header to our list.
                headers.Add(header);
            }
            return headers.ToArray();
        }

        // Parses the array of header strings, and returns the values in a 
        // dictionary for quick hashtable lookups.
        public static Dictionary<string, string> ParseHeaders(string[] headers, int startIndex, int endIndex)
        {
            Dictionary<string, string> headerDict = new Dictionary<string, string>();

            for (int i = startIndex; i < endIndex; i++)
            {
                Match match = headerPattern.Match(headers[i]);
                if (match.Success && (match.Groups.Count == 3))
                {
                    if (headerDict.ContainsKey(match.Groups[1].Value.ToUpper()))
                    {
                        headerDict.Remove(match.Groups[1].Value.ToUpper());
                    }
                    headerDict.Add(match.Groups[1].Value.ToUpper(), match.Groups[2].Value);
                }
            }
            return headerDict;
        }

        public static Dictionary<string, string> ParseHeaders(string[] headers)
        {
            return ParseHeaders(headers, 0, headers.Length);
        }

        /// <summary>
        ///  Reads a single line from the Stream
        /// </summary>
        /// <param name="stream">The stream to read from</param>
        /// <param name="encoding">The encoding to interpret the text</param>
        /// <returns>The line read, without the newline character(s)</returns>
        public static string ReadLine(Stream stream, Encoding encoding)
        {
            byte[] buffer = new byte[8]; // max character bytes
            int bufferPos = 0;

            StringBuilder line = new StringBuilder(100);
            int c;
            bool lineRead = false;
            Decoder decoder = encoding.GetDecoder();
            while ((c = stream.ReadByte()) != -1)
            {
                if (c == '\n')
                {
                    lineRead = true;

                    // strip any '\r' immediately preceding
                    if ((line.Length > 0) && (line[line.Length - 1] == '\r'))
                    {
                        line.Remove(line.Length - 1, 1);
                    }

                    // and break
                    break;
                }
                if (bufferPos >= buffer.Length)
                    throw new InvalidDataException("Invalid character read from stream.");
                buffer[bufferPos++] = (byte)c;
                if (decoder.GetCharCount(buffer, 0, bufferPos) != 1) continue;
                line.Append(encoding.GetChars(buffer, 0, bufferPos));
                bufferPos = 0;
            }
            if (c == -1 && lineRead == false)
                throw new IOException("Stream shut down while reading line.");

            return line.ToString();
        }
    }

}
