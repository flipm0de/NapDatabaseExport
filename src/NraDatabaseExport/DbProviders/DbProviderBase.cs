using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Provides a base type for database providers.
	/// </summary>
	public abstract class DbProviderBase : IDbProvider
	{
		#region IDbProvider Members

		/// <inheritdoc/>
		public string? DatabaseFileName { get; set; }

		/// <inheritdoc/>
		public string? ServerName { get; set; }

		/// <inheritdoc/>
		public int? Port { get; set; }

		/// <inheritdoc/>
		public string? UserName { get; set; }

		/// <inheritdoc/>
		public string? Password { get; set; }

		/// <inheritdoc/>
		public string? DatabaseName { get; set; }

		/// <inheritdoc/>
		public abstract ValueTask<DbConnection> OpenConnectionAsync(
			CancellationToken cancellationToken = default);

		/// <inheritdoc/>
		public abstract ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(
			DbConnection connection,
			CancellationToken cancellationToken = default);

		/// <inheritdoc/>
		public abstract ValueTask<DbTableListItem[]> ListTablesAsync(
			DbConnection connection, 
			CancellationToken cancellationToken = default);

		/// <inheritdoc/>
		public abstract ValueTask<DbDataReader> ExecuteTableReaderAsync(
			DbConnection connection,
			string tableName, 
			string? ownerName = null,
			CancellationToken cancellationToken = default);

		#endregion

		#region IDisposable Members

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Disposes any unmanaged resources allocated by this object.
		/// </summary>
		~DbProviderBase()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases any resources allocated by this object.
		/// </summary>
		/// <param name="disposing">the flag indicating whether the object is being disposed, or the garbage collector has kicked in</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Executes a reader for the result of a command with a specified text.
		/// </summary>
		/// <param name="connection">the database connection to use</param>
		/// <param name="commandText">the text of the command to execute</param>
		/// <param name="parameters">the database parameters to associate with the command</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the data reader containing the result of the command execution</returns>
		protected async ValueTask<DbDataReader> ExecuteReaderAsync(DbConnection connection, string commandText, IDbDataParameter[] parameters = null, CancellationToken cancellationToken = default)
		{
			if (commandText is null)
			{
				throw new ArgumentNullException(nameof(commandText));
			}

			DbCommand command = CreateCommand(connection, commandText, parameters);

			DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);

			if (reader is null)
			{
				throw new DbProviderException("Could not execute a data reader on the database.");
			}

			// detach the parameters from the command object, so they can be used again.
			command.Parameters.Clear();

			return reader;
		}

		/// <summary>
		/// Creates a database command with the specified text.
		/// </summary>
		/// <param name="connection">the database connection to use</param>
		/// <param name="commandText">the text of the command to create</param>
		/// <param name="parameters">the list of database parameters to add to the command</param>
		/// <returns>the created database command</returns>
		protected abstract DbCommand CreateCommand(DbConnection connection, string commandText, IEnumerable<IDbDataParameter> parameters = null);
	}
}
