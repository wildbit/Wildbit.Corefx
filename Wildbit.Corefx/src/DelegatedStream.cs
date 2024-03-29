// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Wildbit.Corefx
{
    internal class DelegatedStream : Stream
    {
        protected static readonly int SURROGATE_START_VALUE = (byte.MaxValue << 6) & byte.MaxValue;
        protected static readonly int TWO_BYTE_CHAR_VALUE = SURROGATE_START_VALUE;
        protected static readonly int THREE_BYTE_CHAR_VALUE = (byte.MaxValue << 5) & byte.MaxValue;
        protected static readonly int FOUR_BYTE_CHAR_VALUE = (byte.MaxValue << 4) & byte.MaxValue;

        private readonly Stream _stream;
        private readonly NetworkStream _netStream;

        protected DelegatedStream()
        {
        }
        protected DelegatedStream(Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            _stream = stream;
            _netStream = stream as NetworkStream;
        }

        protected Stream BaseStream
        {
            get
            {
                return _stream;
            }
        }

        public override bool CanRead
        {
            get
            {
                return _stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException(Strings.SeekNotSupported);

                return _stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                if (!CanSeek)
                    throw new NotSupportedException(Strings.SeekNotSupported);

                return _stream.Position;
            }
            set
            {
                if (!CanSeek)
                    throw new NotSupportedException(Strings.SeekNotSupported);

                _stream.Position = value;
            }
        }

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (!CanRead)
                throw new NotSupportedException(Strings.ReadNotSupported);

            IAsyncResult result = null;

            if (_netStream != null)
            {
                result = _netStream.BeginRead(buffer, offset, count, callback, state);
            }
            else
            {
                result = _stream.BeginRead(buffer, offset, count, callback, state);
            }
            return result;
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            if (!CanWrite)
                throw new NotSupportedException(Strings.WriteNotSupported);

            IAsyncResult result = null;

            if (_netStream != null)
            {
                result = _netStream.BeginWrite(buffer, offset, count, callback, state);
            }
            else
            {
                result = _stream.BeginWrite(buffer, offset, count, callback, state);
            }
            return result;
        }

        //This calls close on the inner stream
        //however, the stream may not be actually closed, but simpy flushed
        public override void Close()
        {
            _stream.Close();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            if (!CanRead)
                throw new NotSupportedException(Strings.ReadNotSupported);

            int read = _stream.EndRead(asyncResult);
            return read;
        }

        public override void EndWrite(IAsyncResult asyncResult)
        {
            if (!CanWrite)
                throw new NotSupportedException(Strings.WriteNotSupported);

            _stream.EndWrite(asyncResult);
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override Task FlushAsync(CancellationToken cancellationToken)
        {
            return _stream.FlushAsync(cancellationToken);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (!CanRead)
                throw new NotSupportedException(Strings.ReadNotSupported);

            int read = _stream.Read(buffer, offset, count);
            return read;
        }

        public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanRead)
                throw new NotSupportedException(Strings.ReadNotSupported);

            return _stream.ReadAsync(buffer, offset, count, cancellationToken);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (!CanSeek)
                throw new NotSupportedException(Strings.SeekNotSupported);

            long position = _stream.Seek(offset, origin);
            return position;
        }

        public override void SetLength(long value)
        {
            if (!CanSeek)
                throw new NotSupportedException(Strings.SeekNotSupported);

            _stream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (!CanWrite)
                throw new NotSupportedException(Strings.WriteNotSupported);

            _stream.Write(buffer, offset, count);
        }

        public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
        {
            if (!CanWrite)
                throw new NotSupportedException(Strings.WriteNotSupported);

            return _stream.WriteAsync(buffer, offset, count, cancellationToken);
        }
    }
}
