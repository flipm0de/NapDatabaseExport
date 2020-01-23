using System;
using System.Data;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Provides a mechanism for working with a database.
	/// </summary>
	public interface IDbProvider : IDisposable
	{
		/// <summary>
		/// Gets the name of the database provider.
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gets the flag indicating whether the database is file-based or not.
		/// </summary>
		bool UsesDatabaseFile { get; }

		/// <summary>
		/// Gets or sets the name of the file containing the database.
		/// </summary>
		/// <remarks>
		/// Applies to file-databases databases only.
		/// </remarks>
		string DatabaseFileName { get; set; }

		/// <summary>
		/// Gets the flag indicating whether the database is server-based or not.
		/// </summary>
		bool UsesServer { get; }

		/// <summary>
		/// Gets or sets the name of the server.
		/// </summary>
		/// <remarks>
		/// The server name might include an instance name (e.g. Microsoft SQL Server).
		/// </remarks>
		string ServerName { get; set; }

		/// <summary>
		/// Gets the flag indicating whether the database server supports specifying a port or not.
		/// </summary>
		bool UsesPort { get; }

		/// <summary>
		/// Gets or sets the default port for connecting to the database server.
		/// </summary>
		/// <remarks>
		/// Applies to server-based databases only.
		/// </remarks>
		int DefaultPort { get; }

		/// <summary>
		/// Gets or sets the port to use for connecting to the database server.
		/// </summary>
		/// <remarks>
		/// Applies to server-based databases only.
		/// </remarks>
		int Port { get; set; }

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a user name or not.
		/// </summary>
		bool UsesUserName { get; }

		/// <summary>
		/// Gets the flag indicating whether the database requires specifying a user name or not.
		/// </summary>
		bool RequiresUserName { get; }

		/// <summary>
		/// Gets or sets the user name to use for connecting to the database.
		/// </summary>
		string UserName { get; set; }

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a password or not.
		/// </summary>
		bool UsesPassword { get; }

		/// <summary>
		/// Gets the flag indicating whether the database requires specifying a password or not.
		/// </summary>
		bool RequiresPassword { get; }

		/// <summary>
		/// Gets or sets the password to use for connecting to the database.
		/// </summary>
		string Password { get; set; }

		/// <summary>
		/// Gets the flag indicating whether the database allows specifying a database name or not.
		/// </summary>
		bool UsesDatabaseName { get; }

		/// <summary>
		/// Gets or sets the name of the database.
		/// </summary>
		string DatabaseName { get; set; }

		/// <summary>
		/// Connects to the database.
		/// </summary>
		/// <returns><see langword="true"/>, if connected; otherwise - <see langword="false"/></returns>
		/// <exception cref="DbProviderException">A database connection could not be established.</exception>
		void CreateConnection();

		/// <summary>
		/// Gets the list of the names of the databases for the current connection.
		/// </summary>
		/// <returns>the list of database names</returns>
		string[] GetDatabaseNames();

		/// <summary>
		/// Gets the list of tables for the current database.
		/// </summary>
		/// <returns>the list of table names</returns>
		string[] GetTableNames();

		/// <summary>
		/// Gets the data reader for the data in a specified table.
		/// </summary>
		/// <param name="tableName">the name of the table</param>
		/// <returns>the data reader containing the data</returns>
		IDataReader ExecuteTableReader(string tableName);
	}
}
