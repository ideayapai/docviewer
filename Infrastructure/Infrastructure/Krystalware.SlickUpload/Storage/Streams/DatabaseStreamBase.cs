using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.IO;

namespace Krystalware.SlickUpload.Storage.Streams
{
    /// <summary>
    /// Exposes an abstract base class for database streams that use the System.Data interfaces.
    /// </summary>
    public abstract class DatabaseStreamBase : Stream
    {
        IDbConnection _connection;
        IDbTransaction _transaction;

        /// <summary>
        /// Gets the current <see cref="IDbConnection" /> for this stream.
        /// </summary>
        public IDbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        /// <summary>
        /// Gets the current <see cref="IDbTransaction" /> for this stream, or null if no transaction exists.
        /// </summary>
        public IDbTransaction Transaction
        {
            get
            {
                return _transaction;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStreamBase" /> class using the specified connection and no transaction.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" /> for this stream.</param>
        protected DatabaseStreamBase(IDbConnection connection)
            : this(connection, null)
        { }

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseStreamBase" /> class using the specified connection and transaction.
        /// </summary>
        /// <param name="connection">The <see cref="IDbConnection" /> for this stream.</param>
        /// <param name="transaction">The <see cref="IDbTransaction" /> for this stream.</param>
        protected DatabaseStreamBase(IDbConnection connection, IDbTransaction transaction)
        {
            _connection = connection;
            _transaction = transaction;
        }

        /// <inheritdoc />
        /// <summary>
        /// Overridden. <inherited />
        /// </summary>
        public override void Close()
        {
            base.Close();

            if (_transaction != null)
                _transaction.Dispose();

            _connection.Dispose();
        }
    }
}
