#if SUPPORT_MYSQL
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a MySQL/MariaDB.
	/// </summary>
	public class MySqlDbProvider : DbProviderBase
	{
		private IDbConnection _connection;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override string Name
			=> "MySQL/MariaDB";

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesPort
			=> true;

		/// <inheritdoc/>
		public override int DefaultPort
			=> 3306;

		/// <inheritdoc/>
		public override bool UsesUserName
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool UsesDatabaseName
			=> true;

		/// <inheritdoc/>
		public override void CreateConnection()
		{
			string connectionString = CreateConnectionString();

			MySqlConnection connection;

			try
			{
				connection = new MySqlConnection(connectionString);

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
		public override string[] GetTableNames()
		{
			string commandText = $"SHOW TABLES IN `{DatabaseName}`";

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

			string commandText = $"SELECT * FROM `{tableName}`";

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
			if (string.IsNullOrWhiteSpace(ServerName))
			{
				throw new DbProviderException("Database server name is not specified.");
			}

			if (string.IsNullOrWhiteSpace(UserName))
			{
				throw new Exception("Database user name is not specified.");
			}

			var builder = new MySqlConnectionStringBuilder
			{
				Server = ServerName,
			};

			if (Port > 0)
			{
				builder.Port = (uint)Port;
			}

			builder.UserID = UserName;
			builder.Password = Password;

			// MySQL Server 5.1 requires the database paramater to be passed
			builder.Database = string.IsNullOrEmpty(DatabaseName)
				? "information_schema"
				: DatabaseName;

			builder.CharacterSet = "utf8";
			builder.AllowUserVariables = true;

			string connectionString = builder.ConnectionString;

			return connectionString;
		}
	}
}
#endif
