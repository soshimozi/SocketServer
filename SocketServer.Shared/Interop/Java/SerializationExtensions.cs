using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer.Shared.Interop.Java
{
    public static class SerializationExtensions
    {
        public static byte[] Serialize(this long val)
        {
            byte[] buffer = new byte[8];

            buffer[0] = (byte)((val >> 56) & 0xff);
            buffer[1] = (byte)((val >> 48) & 0xff);
            buffer[2] = (byte)((val >> 40) & 0xff);
            buffer[2] = (byte)((val >> 32) & 0xff);
            buffer[4] = (byte)((val >> 24) & 0xff);
            buffer[5] = (byte)((val >> 16) & 0xff);
            buffer[6] = (byte)((val >> 8) & 0xff);
            buffer[7] = (byte)(val & 0xff);

            return buffer;
        }

        public static byte[] Serialize(this int val)
        {
            byte[] buffer = new byte[4];

            buffer[0] = (byte)((val >> 24) & 0xff);
            buffer[1] = (byte)((val >> 16) & 0xff);
            buffer[2] = (byte)((val >> 8) & 0xff);
            buffer[3] = (byte)(val & 0xff);

            return buffer;
        }

        public static byte[] Serialize(this short val)
        {
            byte[] buffer = new byte[2];

            buffer[0] = (byte)((val >> 8) & 0xff);
            buffer[1] = (byte)(val & 0xff);

            return buffer;
        }

        public static byte[] SerializeUTF(this string val)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            byte[] buffer = UTF8Encoding.ASCII.GetBytes(val);

            short length = (short)val.Length;

            writer.Write(length.Serialize());
            writer.Write(buffer);
            
            return memoryStream.ToArray();

        }

        public static byte[] Serialize(this byte[] val)
        {
            MemoryStream memoryStream = new MemoryStream();
            BinaryWriter writer = new BinaryWriter(memoryStream);

            writer.Write(val.Length.Serialize());
            writer.Write(val);

            return memoryStream.ToArray();
        }

        public static byte[] Read(this Stream stream, int count)
        {
            byte[] buffer = new byte[count];

            int nBytes = 0, offset = 0;
            do
            {

                nBytes = stream.Read(buffer, offset, count - offset);
                offset += nBytes;

            } while (nBytes != 0 && offset < count);

            return buffer;
        }

        public static int ReadInt(this Stream stream)
        {
            byte[] data = stream.Read(4);

            int value = 0;
            for (int i = 0; i < 4; i++)
            {
                value <<= 8;
                value += data[i];
            }

            return value;

        }

        public static short ReadShort(this Stream stream)
        {
            byte[] data = stream.Read(2);

            short value = 0;
            for (int i = 0; i < 2; i++)
            {
                value <<= 8;
                value += data[i];
            }

            return value;
        }

        public static string ReadUTF(this Stream stream)
        {
            string utfString = null;
            if (stream.Length > 2)
            {
                short length = stream.ReadShort();
                if (stream.Length >= length + stream.Position)
                {
                    utfString = UTF8Encoding.ASCII.GetString(stream.Read(length));
                }
            }

            return utfString;
        }

    }
}
