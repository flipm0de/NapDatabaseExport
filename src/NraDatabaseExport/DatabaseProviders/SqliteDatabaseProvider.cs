#if SUPPORT_SQLITE
using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.Sqlite;

namespace NraDatabaseExport.DatabaseProviders
{
	public class SqliteDatabaseProvider : DatabaseProviderBase
	{
		//private const int BUSY_TIMEOUT = 10 * 1000;

		#region DatabaseProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> "SQLite";

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> true;

		/// <inheritdoc/>
		public override bool TryConnect()
			=> true;

		/// <inheritdoc/>
		public override string[] GetDatabases()
			=> Array.Empty<string>();

		/// <inheritdoc/>
		public override string[] GetTables()
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
		public override IDataReader ExecuteReader(string commandText, params DatabaseParameter[] parameters)
		{
			IDataReader reader;

			using (SqliteCommand command = CreateCommand(commandText, parameters))
			{
				reader = command.ExecuteReader();
				if (reader is null)
				{
					throw new DatabaseProviderException("Could not get a valid reader from the database");
				}

				// detach the SqlParameters from the command object, so they can be used again.
				command.Parameters.Clear();
			}

			return reader;
		}

		#endregion

		private string CreateConnectionString()
		{
			var builder = new SqliteConnectionStringBuilder
			{
				DataSource = DatabaseFile,
				//Pooling = true,
				//Version = 3,
				//DefaultTimeout = BUSY_TIMEOUT,
			};

			return builder.ConnectionString;
		}

		private SqliteConnection CreateConnection()
		{
			try
			{
				var connection = new SqliteConnection(CreateConnectionString());

				connection.Open();

				return connection;
			}
			catch (SqliteException ex)
			{
				throw new DatabaseProviderException("Could not connect to an SQLite database.", ex);
			}
		}

		private SqliteCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			var connection = CreateConnection();

			if (connection is null)
			{
				return null;
			}

			SqliteCommand command = connection.CreateCommand();

			command.CommandText = commandText;

			if (parameters != null)
			{
				foreach (DatabaseParameter p in parameters)
				{
					command.Parameters.Add(
						new SqliteParameter(p.ParameterName, p.Value)
						{
							Direction = p.Direction,
						}
					);
				}
			}

			return command;
		}
	}
}
#endif
