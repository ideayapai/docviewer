using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Krystalware.SlickUpload.Storage.Streams
{
	/// <summary>
    /// Exposes a write-only, forward-only stream around an IMAGE column in a record in a SQL Server database. This stream is unbuffered.
	/// </summary>
	public sealed class SqlClientWriteStream : DatabaseStreamBase
	{
		SqlCommand _cmd;

		SqlParameter _dataParam;
		SqlParameter _offsetParam;

		long _position = 0;

        SqlColumnDataType _dataType;

		/// <summary>
		/// Creates a new instance of the <see cref="SqlClientWriteStream" /> class with the specified connection string, table,
		/// data field, id field, and id value.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="idField">The field which identifies the record.</param>
		/// <param name="idValue">The value which identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientWriteStream(string connectionString, string table, string dataField, string idField, long idValue, SqlColumnDataType dataType) :
            this(connectionString, table, dataField, idField + "=" + idValue.ToString(), dataType)
		{ }

		/// <summary>
		/// Creates a new instance of the <see cref="SqlClientWriteStream" /> class with the specified connection string, table,
		/// data field, and where criteria.
		/// </summary>
		/// <param name="connectionString">The connection string of the database to use.</param>
		/// <param name="table">The table in which the data is stored.</param>
		/// <param name="dataField">The field in which the data is stored</param>
		/// <param name="whereCriteria">The where criteria that identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientWriteStream(string connectionString, string table, string dataField, string whereCriteria, SqlColumnDataType dataType) :
            this(new SqlConnection(connectionString), null, table, dataField, (IDbCommand cmd) => whereCriteria, dataType)
        { }

        /// <summary>
        /// Creates a new instance of the <see cref="SqlClientWriteStream" /> class with the specified connection, transaction, table,
        /// data field, and where criteria action.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" /> to use.</param>
        /// <param name="transaction">The <see cref="IDbTransaction" /> to use, or null for no transaction.</param>
        /// <param name="table">The table in which the data is stored.</param>
        /// <param name="dataField">The field in which the data is stored</param>
        /// <param name="criteriaAction">The <see cref="BuildWhereCriteriaAction"/> to use to generate criteria that identifies the record.</param>
        /// <param name="dataType">The data type of file data column.</param>
        public SqlClientWriteStream(IDbConnection connection, IDbTransaction transaction, string table, string dataField, BuildWhereCriteriaAction criteriaAction, SqlColumnDataType dataType)
            : base(connection, transaction)
		{
            if (dataType == SqlColumnDataType.FileStream)
                throw new ArgumentException("SqlClientWriteStream only supports IMAGE and VARBINARY(MAX). Use SqlFileStreamWriteStream for FILESTREAM.", "dataType");

            _dataType = dataType;

			// TODO: add buffering
			_cmd = (SqlCommand)Connection.CreateCommand();
            _cmd.Transaction = transaction as SqlTransaction;

            string whereCriteria = criteriaAction(_cmd);

            if (_dataType == SqlColumnDataType.VarBinaryMax)
                _cmd.CommandText = "UPDATE " + table + " SET " + dataField + "=CAST('' AS varbinary(MAX)) WHERE " + whereCriteria;
            else
                _cmd.CommandText = "UPDATE " + table + " SET " + dataField + "=NULL WHERE " + whereCriteria;

            try
            {
                if (Connection.State != ConnectionState.Open)
                    Connection.Open();

                _cmd.ExecuteNonQuery();
            }
            finally
            {
                if (Transaction == null)
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
                    if (Transaction == null)
                        Connection.Close();
                }

                _cmd.CommandText = "UPDATETEXT " + table + "." + dataField + " @ptr @offset NULL @data;";

                //_cmd.CommandText = "DECLARE @ptr binary(16);" +
                //	"SELECT @ptr = TEXTPTR(" + dataField + ")" + 
                //	"FROM " + table + " WHERE " + whereCriteria +
                //";UPDATETEXT " + table + "." + dataField + " @ptr @offset NULL @data;";

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
                // TODO: add update length
                _cmd.CommandText = "UPDATE " + table + " SET " + dataField + " .WRITE(@data, @offset, NULL) WHERE " + whereCriteria;
            }

			_offsetParam = _cmd.CreateParameter();
			_offsetParam.DbType = DbType.Int32;
			_offsetParam.ParameterName = "@offset";
			_offsetParam.Size = 4;
			_cmd.Parameters.Add(_offsetParam);

			_dataParam = _cmd.CreateParameter();
			_dataParam.SqlDbType = SqlDbType.Image;
			_dataParam.ParameterName = "@data";
			_dataParam.Size = 8040;
			_cmd.Parameters.Add(_dataParam);

			/*try
			{
				_cn.Open();
				_cmd.Prepare();
			}
			catch (Exception ex)
			{
				ex.ToString();
			}
			finally
			{
				_cn.Close();
			}*/
		}

		/// <summary>
		/// Overridden. Writes a block of bytes to this stream using data from a buffer.
		/// </summary>
		/// <param name="buffer">The array to which bytes are written.</param>
		/// <param name="start">The byte offset in buffer at which to begin writing.</param>
		/// <param name="count">The maximum number of bytes to write.</param>
		public override void Write(byte[] buffer, int start, int count)
		{
			int insertSize = 8040;
			int insertedCount = 0;

			_dataParam.Value = buffer;

			try
			{
                if (Connection.State != ConnectionState.Open)
				    Connection.Open();

				while (insertedCount < count)
				{
					if (insertSize > count - insertedCount)
						insertSize = count - insertedCount;

					_dataParam.Offset = start + insertedCount;
					_dataParam.Size = insertSize;

					_offsetParam.Value = _position;
						
					_cmd.ExecuteNonQuery();

					_position += insertSize;

					insertedCount += insertSize;
				}
			}
			finally
			{
                if (Transaction == null)
				    Connection.Close();
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports reading.
		/// </summary>
		public override bool CanRead
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports seeking.
		/// </summary>
		public override bool CanSeek
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Overridden. Gets a value indicating whether the current stream supports writing.
		/// </summary>
		public override bool CanWrite
		{
			get
			{
				return true;
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
				return _position;
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
				throw new NotSupportedException();
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
			throw new NotSupportedException();
		}

		/// <summary>
		/// Overridden. Sets the current position of this stream to the given value.
		/// </summary>
		/// <param name="offset">The point relative to origin from which to begin seeking. </param>
		/// <param name="origin">Specifies the beginning, the end, or the current position as a reference point for origin, using a value of type <see cref="SeekOrigin"/>. </param>
		/// <returns>The new position in the stream.</returns>
		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
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