#if SUPPORT_MYSQL
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a MySQL/MariaDB.
	/// </summary>
	[DbProvider(DbProviderType.MySql, "MySQL/MariaDB",
		ServerCapability = ProviderCapabilityFlags.SupportedAndRequired,
		PortCapability = ProviderCapabilityFlags.Supported,
		UserNameCapability = ProviderCapabilityFlags.Supported,
		PasswordCapability = ProviderCapabilityFlags.Supported,
		DatabaseCapability = ProviderCapabilityFlags.SupportedAndRequired)]
	public class MySqlDbProvider : DbProviderBase
	{
		#region DbProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
		{
			string connectionString = CreateConnectionString();

			MySqlConnection connection;

			try
			{
				connection = new MySqlConnection(connectionString);

				await connection.OpenAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			return connection;
		}

		/// <inheritdoc/>
		public override async ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			const string commandText = "SHOW DATABASES";

			var listItems = new List<DbDatabaseListItem>();

			DbDataReader reader = await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);

			while (await reader.ReadAsync(cancellationToken: cancellationToken))
			{
				string name = reader.GetString(0);

				switch (name)
				{
					case "information_schema":
					case "performance_schema":
					case "mysql":
						{
							break;
						}
					default:
						{
							var listItem = new DbDatabaseListItem(name);

							listItems.Add(listItem);
							break;
						}
				}
			}

			return listItems.ToArray();
		}

		/// <inheritdoc/>
		public override async ValueTask<DbTableListItem[]> ListTablesAsync(DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			string commandText = $"SHOW TABLES";

			var listItems = new List<DbTableListItem>();

			DbDataReader reader = await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);

			while (await reader.ReadAsync(cancellationToken: cancellationToken))
			{
				string tableName = reader.GetString(0);

				var listItem = new DbTableListItem(tableName);

				listItems.Add(listItem);
			}

			return listItems.ToArray();
		}

		/// <inheritdoc/>
		public override async ValueTask<DbDataReader> ExecuteTableReaderAsync(DbConnection connection, string tableName, string? ownerName = null, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			if (tableName is null)
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			if (!(ownerName is null))
			{
				throw new NotSupportedException("Table owners are not supported by this database provider.");
			}

			string qualifiedTableName = $"`{tableName}`";

			string commandText = $"SELECT * FROM {qualifiedTableName}";

			return await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);
		}

		/// <inheritdoc/>
		protected override DbCommand CreateCommand(DbConnection connection, string commandText, IEnumerable<IDbDataParameter> parameters = null)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			DbCommand command = connection.CreateCommand();

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
				Port = Port is null
					? 3306
					: Port <= 0
						? 3306
						: (uint)Port.Value,
				UserID = UserName,
				Password = Password,
			};

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
