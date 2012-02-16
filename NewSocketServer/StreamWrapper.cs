using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SocketServer
{
    public class StreamWrapper : Stream
    {
        #region Fields

        private readonly MemoryStream inStream;
        private readonly MemoryStream outStream;

        private bool disposed;

        #endregion Fields

        #region Constructors

        public StreamWrapper(Stream stream)
        {
            Stream = stream;
            inStream = new MemoryStream();
            outStream = new MemoryStream();
        }

        #endregion Constructors

        #region Properties

        public override bool CanRead
        {
            get { return Stream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return Stream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return Stream.CanWrite; }
        }

        public override long Length
        {
            get { return Stream.Length; }
        }

        public override long Position
        {
            get
            {
                return Stream.Position;
            }
            set
            {
                Stream.Position = value;
            }
        }

        public Stream Stream
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public override void Flush()
        {
            Stream.Flush();
        }

        public byte[] GetInputBytes()
        {
            return inStream.ToArray();
        }

        public byte[] GetOutputBytes()
        {
            return outStream.ToArray();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            byte[] data = new byte[count];
            // Read into our temp data structure
            int read = Stream.Read(data, 0, count);
            // Make sure we write this data into the copy buffer
            inStream.Write(data, 0, read);
            // now copy this data into the output buffer
            data.CopyTo(buffer, offset);
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return Stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            Stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            Stream.Write(buffer, offset, count);
            outStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing && !disposed)
            {
                inStream.Dispose();
                outStream.Dispose();
                disposed = true;
            }
        }

        #endregion Methods
    }

}
