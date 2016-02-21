using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Data.SqlTypes;

namespace Krystalware.SlickUpload.Storage.Streams
{
	/// <summary>
    /// Exposes a read-only stream around a FILESTREAM column in a record in a SQL Server database. This stream is unbuffered.
	/// </summary>
    public sealed class SqlFileStreamReadStream : DatabaseStreamBase
	{
        SqlFileStream _innerStream;

		/// <summary>
        /// Creates a new instance of the <see cref="SqlFileStreamReadStream" /> class with the specified connection string, table,
		/// data field, id field, and id value.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="idField">The field which identifies the record.</param>
		/// <param name="idValue">The value which identifies the record.</param>
		public SqlFileStreamReadStream(string connectionString, string table, string dataField, string idField, int idValue) : 
			this(connectionString, table, dataField, idField + "=" + idValue.ToString())
		{}

		/// <summary>
        /// Creates a new instance of the <see cref="SqlFileStreamReadStream" /> class with the specified connection string, table,
		/// data field, and where criteria.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="whereCriteria">The where criteria that identifies the record.</param>
        public SqlFileStreamReadStream(string connectionString, string table, string dataField, string whereCriteria) :
            this(new SqlConnection(connectionString), table, dataField, (IDbCommand cmd) => whereCriteria)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlFileStreamReadStream" /> class with the specified connection string, table,
        /// data field, and where criteria action.
        /// </summary>
        /// <param name="connectionString">The connection string of the database to use.</param>
        /// <param name="table">The table in which the data is stored.</param>
        /// <param name="dataField">The field in which the data is stored</param>
        /// <param name="criteriaAction">The <see cref="BuildWhereCriteriaAction"/> to use to generate criteria that identifies the record.</param>
        public SqlFileStreamReadStream(string connectionString, string table, string dataField, BuildWhereCriteriaAction criteriaAction) :
            this(new SqlConnection(connectionString), table, dataField, criteriaAction)
        { }
        
        /// <summary>
        /// Creates a new instance of the <see cref="SqlFileStreamReadStream" /> class with the specified connection, table,
        /// data field, and where criteria action.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <param name="table">The table in which the data is stored.</param>
        /// <param name="dataField">The field in which the data is stored</param>
        /// <param name="criteriaAction">The <see cref="BuildWhereCriteriaAction"/> to use to generate criteria that identifies the record.</param>
        public SqlFileStreamReadStream(IDbConnection connection, string table, string dataField, BuildWhereCriteriaAction criteriaAction)
            : base(connection, OpenConnectionAndBeginTransaction(connection))
        {
            // TODO: add buffering

            try
            {
                using (SqlCommand cmd = ((SqlConnection)Connection).CreateCommand())
                {
                    cmd.CommandText = "SELECT " + dataField + ".PathName(), GET_FILESTREAM_TRANSACTION_CONTEXT() FROM " + table + " WHERE " + criteriaAction(cmd);
                    cmd.Transaction = (SqlTransaction)Transaction;

                    using (SqlDataReader rd = cmd.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        rd.Read();

                        string path = rd.GetString(0);
                        byte[] context = (byte[])rd.GetValue(1);

                        // todo: fileoptions, allocationsize
                        _innerStream = new SqlFileStream(path, context, FileAccess.Read);
                    }
                }
            }
            catch
            {
                if (_innerStream != null)
                    _innerStream.Dispose();

                base.Close();

                // TODO: cleanup
                throw;
            }
        }

        static IDbTransaction OpenConnectionAndBeginTransaction(IDbConnection cn)
        {
            try
            {
                cn.Open();

                return cn.BeginTransaction(IsolationLevel.ReadCommitted);
            }
            catch
            {
                cn.Dispose();

                throw;
            }
        }

		/// <summary>
		/// Overridden. Writes a block of bytes to this stream using data from a buffer.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written.</param>
		/// <param name="offset">The byte offset in buffer at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
            _innerStream.Write(buffer, offset, count);
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return _innerStream.CanRead;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return _innerStream.CanSeek;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return _innerStream.CanWrite;
			}
		}

		/// <summary>
		/// Overridden. Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
            _innerStream.Flush();
		}

		/// <summary>
		/// Overridden. Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get
			{
				return _innerStream.Length;
			}
		}

		/// <summary>
		/// Overridden. Gets or sets the current position of this stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return _innerStream.Position;
			}
			set
			{
                _innerStream.Position = value;
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
            return _innerStream.Read(buffer, offset, count);
		}

		/// <summary>
		/// Overridden. Sets the current position of this stream to the given value.
		/// </summary>
		/// <param name="offset">The point relative to origin from which to begin seeking.</param>
		/// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for origin, using a value of type <see cref="SeekOrigin"/>. </param>
		/// <returns>The new position in the stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
            return _innerStream.Seek(offset, origin);
		}

		/// <summary>
		/// Overridden. Sets the length of this stream to the given value.
		/// </summary>
		/// <param name="value">The new length of the stream.</param>
		public override void SetLength(long value)
		{
            _innerStream.SetLength(value);
		}

		/// <summary>
		/// Overridden. Closes the file and releases any resources associated with the current stream.
		/// </summary>
		public override void Close()
		{
            _innerStream.Dispose();
            
            base.Close();
		}
	}
}