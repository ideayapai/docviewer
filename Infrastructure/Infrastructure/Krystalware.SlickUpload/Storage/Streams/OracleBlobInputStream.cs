using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Krystalware.SlickUpload.Storage;

namespace Krystalware.SlickUpload.Storage.Streams
{
	/// <summary>
    /// Exposes a write-only, forward-only stream around an image column in a record in a SQL Server database. This stream is unbuffered.
	/// </summary>
	public sealed class OracleBlobInputStream : Stream
	{
		IDbConnection _cn;
		IDbCommand _cmd;

		ReflectWrapper _blob;

		/// <summary>
		/// Creates a new instance of the <see cref="SqlClientInputStream" /> class for the specified connection string, table
		/// data field, id field, and id value.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="idField">The field which identifies the record.</param>
		/// <param name="idValue">The value which identifies the record.</param>
		/*public SqlClientInputStream(string connectionString, string table, string dataField, string idField, int idValue) : 
			this(connectionString, table, dataField, idField + "=" + idValue.ToString())
		{}*/

		/// <summary>
		/// Creates a new instance of the <see cref="SqlClientInputStream" /> class for the specified connection string, table
		/// data field, and where criteria.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="whereCriteria">The where criteria that identifies the record.</param>
		public OracleBlobInputStream(string connectionString, string table, string dataField, string whereCriteria)
		{
			// TODO: add buffering
            _cn = OracleBlobUploadStreamProvider.CreateConnection(connectionString);

            _cmd = _cn.CreateCommand();
            _cmd.CommandText = "UPDATE " + table + " SET " + dataField + "=:blob WHERE " + whereCriteria;

            _cn.Open();

            object blob = TypeCache.CreateInstance("Oracle.DataAccess.Types.OracleBlob, " + OracleBlobUploadStreamProvider._odpAssembly, new object[] { _cn });

            _blob = new ReflectWrapper(blob);

            _blob.InvokeVoid("BeginChunkWrite");
        }

        /// <summary>
        /// Overridden. Writes a block of bytes to this stream using data from a buffer.
        /// </summary>
        /// <param name="buffer">The array to which bytes are written.</param>
        /// <param name="offset">The byte offset in buffer at which to begin writing.</param>
        /// <param name="count">The maximum number of bytes to write.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _blob.InvokeVoid("Write", buffer, offset, count);
        }

        /// <summary>
        /// Overridden. Gets a value indicating whether the current stream supports reading.
        /// </summary>
        public override bool CanRead
        {
            get
            {
                return _blob.GetProp<bool>("CanRead");
            }
        }

        /// <summary>
        /// Overridden. Gets a value indicating whether the current stream supports seeking.
        /// </summary>
        public override bool CanSeek
        {
            get
            {
                return _blob.GetProp<bool>("CanSeek");
            }
        }

        /// <summary>
        /// Overridden. Gets a value indicating whether the current stream supports writing.
        /// </summary>
        public override bool CanWrite
        {
            get
            {
                return _blob.GetProp<bool>("CanWrite");
            }
        }

        /// <summary>
        /// Overridden. Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
        /// </summary>
        public override void Flush()
        {
            _blob.InvokeVoid("Flush");
        }

        /// <summary>
        /// Overridden. Gets the length in bytes of the stream.
        /// </summary>
        public override long Length
        {
            get
            {
                return _blob.GetProp<long>("Length");
            }
        }

        /// <summary>
        /// Overridden. Gets or sets the current position of this stream.
        /// </summary>
        public override long Position
        {
            get
            {
                return _blob.GetProp<long>("Position");
            }
            set
            {
                _blob.SetProp("Position", value);
            }
        }

        /// <summary>
        /// Overridden. Reads a block of bytes from the stream and writes the data in a given buffer.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in buffer at which to begin reading.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes read into the buffer. This might be less than the number of bytes requested if that number of bytes are not currently available, or zero if the end of the stream is reached.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _blob.Invoke<int>("Read", buffer, offset, count);
        }

        /// <summary>
        /// Overridden. Sets the current position of this stream to the given value.
        /// </summary>
        /// <param name="offset">The point relative to origin from which to begin seeking.</param>
        /// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for origin, using a value of type <see cref="SeekOrigin"/>. </param>
        /// <returns>The new position in the stream.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _blob.Invoke<long>("Seek", offset, origin);
        }

        /// <summary>
        /// Overridden. Sets the length of this stream to the given value.
        /// </summary>
        /// <param name="value">The new length of the stream.</param>
        public override void SetLength(long value)
        {
            _blob.InvokeVoid("SetLength", value);
        }

		/// <summary>
		/// Overridden. Closes the file and releases any resources associated with the current file stream.
		/// </summary>
		public override void Close()
		{
			base.Close();

            _blob.InvokeVoid("EndChunkWrite");

            IDbDataParameter blobParm = _cmd.CreateParameter();

            blobParm.ParameterName = ":blob";
            blobParm.Direction = ParameterDirection.Input;
            blobParm.GetType().GetProperty("OracleDbType").SetValue(blobParm, 102, null);
            blobParm.Value = _blob.Target;

            _cmd.Parameters.Add(blobParm);

            _cmd.ExecuteNonQuery();
            
            _blob.InvokeVoid("Dispose");
			_cn.Dispose();
            _cmd.Dispose();
		}
	}
}