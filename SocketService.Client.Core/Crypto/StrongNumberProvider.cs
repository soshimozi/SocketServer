using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SocketService.Framework.Client.Crypto
{
    [Serializable]
    public class StrongNumberProvider
    {
        private static RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();

        /// <summary>
        /// Gets the next U int32.
        /// </summary>
        /// <returns></returns>
        public uint GetNextUInt32()
        {
            byte[] res = new byte[4];
            csp.GetBytes(res);
            return BitConverter.ToUInt32(res, 0);
        }

        /// <summary>
        /// Gets the next int.
        /// </summary>
        /// <returns></returns>
        public int GetNextInt()
        {
            byte[] res = new byte[4];
            csp.GetBytes(res);
            return BitConverter.ToInt32(res, 0);
        }

        /// <summary>
        /// Gets the next single.
        /// </summary>
        /// <returns></returns>
        public Single GetNextSingle()
        {
            float numerator = GetNextUInt32();
            float denominator = uint.MaxValue;
            return numerator / denominator;
        }
    }
}
