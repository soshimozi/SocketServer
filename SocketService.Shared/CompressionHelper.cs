using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.IO.Compression;

namespace TestSocketServer.Utilities
{
    public static class CompressionHelper
    {
        public static byte[] DeCompress(byte[] data)
        {
            using (var msData = new MemoryStream())
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var gz = new GZipStream(msData, CompressionMode.Decompress))
                    {
                        var bytes = new byte[4096];
                        int n;
                        while ((n = ms.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            gz.Write(bytes, 0, n);
                        }
                    }
                }

                return msData.ToArray();
            }
        }

        public static byte[] Compress(byte[] data)
        {
            using (var msData = new MemoryStream())
            {
                using (var ms = new MemoryStream(data))
                {
                    using (var gz = new GZipStream(msData, CompressionMode.Compress))
                    {
                        var bytes = new byte[4096];
                        int n;
                        while ((n = ms.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            gz.Write(bytes, 0, n);
                        }
                    }
                }

                return msData.ToArray();
            }

        }

    }
}
