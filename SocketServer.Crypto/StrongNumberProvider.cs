using System;
using System.Security.Cryptography;

namespace SocketServer.Crypto
{
    [Serializable]
    public class StrongNumberProvider
    {
        private static readonly RNGCryptoServiceProvider Csp = new RNGCryptoServiceProvider();

        /// <summary>
        /// Gets the next U int32.
        /// </summary>
        /// <returns></returns>
        public uint GetNextUInt32()
        {
            var res = new byte[4];
            Csp.GetBytes(res);
            return BitConverter.ToUInt32(res, 0);
        }

        /// <summary>
        /// Gets the next int.
        /// </summary>
        /// <returns></returns>
        public int GetNextInt()
        {
            var res = new byte[4];
            Csp.GetBytes(res);
            return BitConverter.ToInt32(res, 0);
        }

        /// <summary>
        /// Gets the next single.
        /// </summary>
        /// <returns></returns>
        public Single GetNextSingle()
        {
            float numerator = GetNextUInt32();
            const float denominator = uint.MaxValue;
            return numerator / denominator;
        }
    }
}
