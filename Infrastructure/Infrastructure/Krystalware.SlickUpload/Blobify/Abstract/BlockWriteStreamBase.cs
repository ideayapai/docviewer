using System;
using System.IO;

namespace Krystalware.SlickUpload.Blobify.Abstract
{
    public abstract class BlockWriteStreamBase : Stream
    {
        long _streamPos;
        bool _isAborted;
        bool _isCompleted;
        bool _isInitialized;

        MemoryStream _bufferStream = new MemoryStream();
        int _blockSize;

        public BlockWriteStreamBase(int blockSize)
        {
            _blockSize = blockSize;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_isAborted)
                throw new InvalidOperationException("Cannot write to an aborted stream.");

            if (_isCompleted)
                throw new InvalidOperationException("Cannot write to a completed stream.");

            if (!_isInitialized)
            {
                // TODO: only initialize if we're writing a block to the underlying source. that way, we can use an
                // all in one go method if the entire stream ends up being less than the block size
                Initialize();

                _isInitialized = true;
            }

            int writeLength;

            // TODO: should we use everything that memorystream allocates?
            while ((writeLength = Math.Min(count, _blockSize - (int)_bufferStream.Position)) != 0)
            {
                _bufferStream.Write(buffer, offset, writeLength);

                _streamPos += writeLength;

                offset += writeLength;
                count -= writeLength;

                if (_bufferStream.Position == _blockSize)
                {
                    WriteBlock(_bufferStream.GetBuffer(), 0, (int)_bufferStream.Position);

                    _bufferStream.Position = 0;
                }
            }
        }

        protected abstract void WriteBlock(byte[] data, int offset, int count);
        protected abstract void Initialize();
        protected abstract void CompleteInternal();
        protected abstract void AbortInternal();

        void Complete()
        {
            if (_isCompleted)
                throw new InvalidOperationException("Cannot complete a completed stream.");
            else if (_isAborted)
                throw new InvalidOperationException("Cannot complete an aborted stream.");

            if (_isInitialized && !_isAborted && _bufferStream.Position > 0)
                WriteBlock(_bufferStream.GetBuffer(), 0, (int)_bufferStream.Position);

            CompleteInternal();

            _isCompleted = true;
        }

        public void Abort()
        {
            if (_isInitialized)
            {
                if (_isCompleted)
                    throw new InvalidOperationException("Cannot abort a completed stream.");
                else if (_isAborted)
                    throw new InvalidOperationException("Cannot abort an aborted stream.");


                AbortInternal();
            }

            _isAborted = true;
        }

        public override void Close()
        {
            if (_isInitialized && !_isAborted && !_isCompleted)
                Complete();

            _bufferStream.Dispose();

            base.Close();
        }

        public override bool CanRead
        {
            get { return false; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override long Length
        {
            get { throw new NotImplementedException(); }
        }

        public override long Position
        {
            get
            {
                return _streamPos;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }
    }
}
