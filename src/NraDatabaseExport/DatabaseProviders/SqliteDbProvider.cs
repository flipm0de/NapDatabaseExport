#if SUPPORT_SQLITE
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for SQLite.
	/// </summary>
	public class SqliteDbProvider : DbProviderBase
	{
		private IDbConnection _connection;

		//private const int BUSY_TIMEOUT = 10 * 1000;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override string Name
			=> "SQLite";

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> true;

		/// <inheritdoc/>
		public override void CreateConnection()
		{
			string connectionString = CreateConnectionString();

			SqliteConnection connection;

			try
			{
				connection = new SqliteConnection(connectionString);

				connection.Open();
			}
			catch (Exception ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			_connection = connection;
		}

		/// <inheritdoc/>
		public override string[] GetDatabaseNames()
			=> Array.Empty<string>();

		/// <inheritdoc/>
		public override string[] GetTableNames()
		{
			const string commandText = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name";

			var tables = new List<string>();

			using (IDataReader reader = ExecuteReader(commandText))
			{
				while (reader.Read())
				{
					tables.Add(reader.GetString(0));
				}
			}

			return tables.ToArray();
		}

		/// <inheritdoc/>
		public override IDataReader ExecuteTableReader(string tableName)
		{
			if (tableName is null)
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			string commandText = $"SELECT * FROM {tableName}";

			return ExecuteReader(commandText);
		}

		/// <inheritdoc/>
		protected override IDbCommand CreateCommand(string commandText, IEnumerable<IDbDataParameter> parameters = null)
		{
			if (_connection is null)
			{
				throw new DbProviderException($"The database connection is not opened.");
			}

			IDbCommand command = _connection.CreateCommand();

			command.CommandText = commandText;

			if (parameters != null)
			{
				foreach (IDbDataParameter parameter in parameters)
				{
					command.Parameters.Add(parameter);
				}
			}

			return command;
		}

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_connection?.Dispose();
			}

			_connection = null;

			base.Dispose(disposing);
		}

		#endregion

		private string CreateConnectionString()
		{
			if (string.IsNullOrWhiteSpace(DatabaseFileName))
			{
				throw new DbProviderException("Database file name is not specified.");
			}

			var builder = new SqliteConnectionStringBuilder
			{
				DataSource = DatabaseFileName,
				//Pooling = true,
				//Version = 3,
				//DefaultTimeout = BUSY_TIMEOUT,
			};

			return builder.ConnectionString;
		}
	}
}
#endif
