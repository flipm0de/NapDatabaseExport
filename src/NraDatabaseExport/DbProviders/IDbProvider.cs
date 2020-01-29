using System;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Provides a mechanism for working with a database.
	/// </summary>
	public interface IDbProvider : IDisposable
	{
		/// <summary>
		/// Gets or sets the name of the file containing the database.
		/// </summary>
		/// <remarks>
		/// Applies to file-databases databases only.
		/// </remarks>
		string? DatabaseFileName { get; set; }

		/// <summary>
		/// Gets or sets the name of the server.
		/// </summary>
		/// <remarks>
		/// The server name might include an instance name (e.g. Microsoft SQL Server).
		/// </remarks>
		string? ServerName { get; set; }

		/// <summary>
		/// Gets or sets the port to use for connecting to the database server.
		/// </summary>
		/// <remarks>
		/// Applies to server-based databases only.
		/// </remarks>
		int? Port { get; set; }

		/// <summary>
		/// Gets or sets the user name to use for connecting to the database.
		/// </summary>
		string? UserName { get; set; }

		/// <summary>
		/// Gets or sets the password to use for connecting to the database.
		/// </summary>
		string? Password { get; set; }

		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		string? DatabaseName { get; set; }

		/// <summary>
		/// Opens a connection to the database.
		/// </summary>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of opening a connection to the database</returns>
		/// <exception cref="DbProviderException">A database connection could not be established.</exception>
		ValueTask<DbConnection> OpenConnectionAsync(
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the list of the names of the databases for the current connection.
		/// </summary>
		/// <param name="connection">the database connection to use</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of listing the databases</returns>
		ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(
			DbConnection connection,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the list of tables (for the current database).
		/// </summary>
		/// <param name="connection">the database connection to use</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of listing tables</returns>
		ValueTask<DbTableListItem[]> ListTablesAsync(
			DbConnection connection,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Gets the data reader for the data in a specified table.
		/// </summary>
		/// <param name="connection">the database connection to use</param>
		/// <param name="tableName">the name of the table</param>
		/// <param name="ownerName">the name of the owner (schema) of the table</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of executing a table reader for data in the table</returns>
		ValueTask<DbDataReader> ExecuteTableReaderAsync(
			DbConnection connection,
			string tableName,
			string? ownerName = null,
			CancellationToken cancellationToken = default);
	}
}
