using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Krystalware.SlickUpload.Storage.Streams
{
	/// <summary>
    /// Exposes a read-only stream around an IMAGE column in a record in a SQL Server database. This stream is unbuffered.
	/// </summary>
	public sealed class SqlClientReadStream : DatabaseStreamBase
	{
		SqlCommand _cmd;

		SqlParameter _sizeParam;
		SqlParameter _offsetParam;

		long _position = 0;
		long _length = 0;

        SqlColumnDataType _dataType;

		/// <summary>
        /// Creates a new instance of the <see cref="SqlClientReadStream" /> class with the specified connection string, table,
		/// data field, id field, and id value.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="idField">The field which identifies the record.</param>
		/// <param name="idValue">The value which identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientReadStream(string connectionString, string table, string dataField, string idField, long idValue, SqlColumnDataType dataType) :
            this(connectionString, table, dataField, idField + "=" + idValue.ToString(), dataType)
		{ }

		/// <summary>
        /// Creates a new instance of the <see cref="SqlClientReadStream" /> class with the specified connection string, table,
		/// data field, and where criteria.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="whereCriteria">The where criteria that identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientReadStream(string connectionString, string table, string dataField, string whereCriteria, SqlColumnDataType dataType) :
            this(new SqlConnection(connectionString), table, dataField, (IDbCommand cmd) => whereCriteria, dataType)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlClientReadStream" /> class with the specified connection string, table,
        /// data field, and where criteria action.
        /// </summary>
        /// <param name="connectionString">The connection string of the database to use.</param>
        /// <param name="table">The table in which the data is stored.</param>
        /// <param name="dataField">The field in which the data is stored</param>
        /// <param name="criteriaAction">The <see cref="BuildWhereCriteriaAction"/> to use to generate criteria that identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientReadStream(string connectionString, string table, string dataField, BuildWhereCriteriaAction criteriaAction, SqlColumnDataType dataType) :
            this(new SqlConnection(connectionString), table, dataField, criteriaAction, dataType)
        { }
        
        /// <summary>
        /// Creates a new instance of the <see cref="SqlClientReadStream" /> class with the specified connection, table,
        /// data field, and where criteria action.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <param name="table">The table in which the data is stored.</param>
        /// <param name="dataField">The field in which the data is stored</param>
        /// <param name="criteriaAction">The <see cref="BuildWhereCriteriaAction"/> to use to generate criteria that identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientReadStream(IDbConnection connection, string table, string dataField, BuildWhereCriteriaAction criteriaAction, SqlColumnDataType dataType)
            : base(connection)
		{
            if (dataType == SqlColumnDataType.FileStream)
                throw new ArgumentException("SqlClientReadStream only supports IMAGE and VARBINARY(MAX). Use SqlFileStreamReadStream for FILESTREAM.", "dataType");

            _dataType = dataType;

			// TODO: add buffering
			_cmd = (SqlCommand)Connection.CreateCommand();

            string whereCriteria = criteriaAction(_cmd);

            _cmd.CommandText = "SELECT CAST(DATALENGTH(" + dataField + ") AS bigint) FROM " + table + " WHERE " + whereCriteria;

			try
			{
                if (Connection.State != ConnectionState.Open)
				    Connection.Open();

				_length = (long)_cmd.ExecuteScalar();
			}
			finally
			{
                Connection.Close();
            }

            if (_dataType != SqlColumnDataType.VarBinaryMax)
            {
			    byte[] ptr;

			    _cmd.CommandText = "SELECT TEXTPTR(" + dataField + ") FROM " + table + " WHERE " + whereCriteria;

			    try
			    {
                    if (Connection.State != ConnectionState.Open)
                        Connection.Open();

				    ptr = (byte[])_cmd.ExecuteScalar();
			    }
			    finally
			    {
                    Connection.Close();
                }

			    _cmd.CommandText = "READTEXT " + table + "." + dataField + " @ptr @offset @size;";

                _cmd.Parameters.Clear();

			    SqlParameter ptrParam = _cmd.CreateParameter();
			    ptrParam.DbType = DbType.Binary;
			    ptrParam.ParameterName = "@ptr";
			    ptrParam.Size = 16;
			    ptrParam.Value = ptr;
			    _cmd.Parameters.Add(ptrParam);
            }
            else
            {
                // TODO: test SqlBinary perf vs SUBSTRING
			    _cmd.CommandText = "SELECT SUBSTRING(" + dataField + ",@offset,@size) FROM " + table + " WHERE " + whereCriteria;
            }
			
			_offsetParam = _cmd.CreateParameter();
            _offsetParam.DbType = (_dataType == SqlColumnDataType.VarBinaryMax) ? DbType.Int64 : DbType.Int32;
			_offsetParam.ParameterName = "@offset";
            _offsetParam.Size = (_dataType == SqlColumnDataType.VarBinaryMax) ? 4 : 8;
			_cmd.Parameters.Add(_offsetParam);

			_sizeParam = _cmd.CreateParameter();
            _sizeParam.DbType = (_dataType == SqlColumnDataType.VarBinaryMax) ? DbType.Int64 : DbType.Int32;
			_sizeParam.ParameterName = "@size";
            _sizeParam.Size = (_dataType == SqlColumnDataType.VarBinaryMax) ? 4 : 8;
			_cmd.Parameters.Add(_sizeParam);
		}

		/// <summary>
		/// Overridden. Writes a block of bytes to this stream using data from a buffer.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written.</param>
		/// <param name="offset">The byte offset in buffer at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException();
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Overridden. Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
		/// </summary>
		public override void Flush()
		{
			
		}

		/// <summary>
		/// Overridden. Gets the length in bytes of the stream.
		/// </summary>
		public override long Length
		{
			get
			{
				return _length;
			}
		}

		/// <summary>
		/// Overridden. Gets or sets the current position of this stream.
		/// </summary>
		public override long Position
		{
			get
			{
				return _position;
			}
			set
			{
				if (_position < 0 || _position > _length)
					throw new ArgumentOutOfRangeException("value");

				_position = value;
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
			int read;

			if (_position > _length)
				throw new InvalidOperationException("Tried to read past end of stream.");
			else if (_position == _length)
				return 0;

			if (_position + count > _length)
				read = (int)(_length - _position);
			else
				read = count;

			_sizeParam.Value = read;

            if (_dataType == SqlColumnDataType.VarBinaryMax)
                _offsetParam.Value = _position + 1;
            else
			    _offsetParam.Value = _position;

			// TODO: buffer
			byte[] tempBuffer = null;

			try
			{
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

				tempBuffer = (byte[])_cmd.ExecuteScalar();
			}
			finally
			{
				Connection.Close();
			}

			Buffer.BlockCopy(tempBuffer, 0, buffer, offset, read);

			_position += read;

			return read;
		}

		/// <summary>
		/// Overridden. Sets the current position of this stream to the given value.
		/// </summary>
		/// <param name="offset">The point relative to origin from which to begin seeking.</param>
		/// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for origin, using a value of type <see cref="SeekOrigin"/>. </param>
		/// <returns>The new position in the stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin:
					if (offset < 0 || offset > _length)
						throw new ArgumentOutOfRangeException("offset");

					_position = offset;

					break;
				case SeekOrigin.End:
					if (offset > 0 || _length + offset < 0)
						throw new ArgumentOutOfRangeException("offset");

					_position = _length + offset;

					break;
				case SeekOrigin.Current:
					if (_position + offset < 0 || _position + offset > _length)
						throw new ArgumentOutOfRangeException("offset");

					_position += offset;

					break;
			}

			return _position;
		}

		/// <summary>
		/// Overridden. Sets the length of this stream to the given value.
		/// </summary>
		/// <param name="value">The new length of the stream.</param>
		public override void SetLength(long value)
		{
			throw new NotSupportedException();			
		}

		/// <summary>
		/// Overridden. Closes the file and releases any resources associated with the current stream.
		/// </summary>
		public override void Close()
		{
            _cmd.Dispose();

			base.Close();
		}
	}
}