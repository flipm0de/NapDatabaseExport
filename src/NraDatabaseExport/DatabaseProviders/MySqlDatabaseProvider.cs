#if SUPPORT_MYSQL
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace NraDatabaseExport.DatabaseProviders
{
	public class MySqlDatabaseProvider : DatabaseProviderBase
	{
		#region DatabaseProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> "MySQL/MariaDB";

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesPort
			=> true;

		/// <inheritdoc/>
		public override uint DefaultPort
			=> 3306;

		/// <inheritdoc/>
		public override bool UsesUser
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool UsesDatabase
			=> true;

		/// <inheritdoc/>
		public override bool TryConnect()
		{
			if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(User))
			{
				throw new Exception("The connection parameters are not properly set.");
			}

			MySqlConnection conn = null;
			try
			{
				conn = CreateConnection();
			}
			finally
			{
				conn?.Close();
			}

			return true;
		}

		/// <inheritdoc/>
		public override string[] GetDatabases()
		{
			const string commandText = "SHOW DATABASES";

			var dbs = new List<string>();

			using (IDataReader reader = ExecuteReader(commandText))
			{
				while (reader.Read())
				{
					dbs.Add(reader.GetString(0));
				}
			}

			dbs.Remove("information_schema");
			dbs.Remove("performance_schema");
			dbs.Remove("mysql");

			return dbs.ToArray();
		}

		/// <inheritdoc/>
		public override string[] GetTables()
		{
			string commandText = $"SHOW TABLES IN `{Database}`";

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

			using (MySqlCommand command = CreateCommand(commandText, parameters))
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
			var connectionString = new MySqlConnectionStringBuilder();

			if (!string.IsNullOrEmpty(Server))
			{
				connectionString.Server = Server;

				if (Port > 0)
				{
					connectionString.Port = Port;
				}
			}

			if (!string.IsNullOrEmpty(User))
			{
				connectionString.UserID = User;
			}

			if (!string.IsNullOrEmpty(Password))
			{
				connectionString.Password = Password;
			}

			// MySQL Server 5.1 requires the database paramater to be passed
			connectionString.Database = string.IsNullOrEmpty(Database)
				? "information_schema"
				: Database;

			connectionString.CharacterSet = "utf8";
			connectionString.AllowUserVariables = true;

			return connectionString.GetConnectionString(true);
		}

		private MySqlConnection CreateConnection()
		{
			try
			{
				var connection = new MySqlConnection(CreateConnectionString());

				connection.Open();

				return connection;
			}
			catch (SocketException ex)
			{
				throw new DatabaseProviderException("Could not connect to a MySQL/MariaDB database.", ex);
			}
			catch (MySqlException ex)
			{
				throw new DatabaseProviderException("Could not connect to a MySQL/MariaDB database.", ex);
			}
		}

		private MySqlCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			var connection = CreateConnection();

			if (connection is null)
			{
				return null;
			}

			MySqlCommand command = connection.CreateCommand();

			command.CommandText = commandText;

			if (parameters != null)
			{
				foreach (DatabaseParameter p in parameters)
				{
					command.Parameters.Add(
						new MySqlParameter(p.ParameterName, p.Value)
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
