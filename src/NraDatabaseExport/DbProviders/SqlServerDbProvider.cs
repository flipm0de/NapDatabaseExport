#if SUPPORT_SQLSERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for Microsoft SQL Server.
	/// </summary>
	[DbProvider(DbProviderType.SqlServer, "Microsoft SQL Server",
		ServerCapability = ProviderCapabilityFlags.SupportedAndRequired,
		PortCapability = ProviderCapabilityFlags.Supported,
		UserNameCapability = ProviderCapabilityFlags.Supported,
		PasswordCapability = ProviderCapabilityFlags.Supported,
		DatabaseCapability = ProviderCapabilityFlags.SupportedAndRequired)]
	public class SqlServerDbProvider : DbProviderBase
	{
		#region DbProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
		{
			string connectionString = CreateConnectionString();

			SqlConnection connection;

			try
			{
				connection = new SqlConnection(connectionString);

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

			const string commandText = "SELECT [name] FROM [master].[sys].[databases] WHERE [database_id] > 4 ORDER BY [name]";

			var listItems = new List<DbDatabaseListItem>();

			using DbDataReader reader = await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);

			while (await reader.ReadAsync(cancellationToken: cancellationToken))
			{
				string name = reader.GetString(0);

				var listItem = new DbDatabaseListItem(name);

				listItems.Add(listItem);
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

			string commandText = $@"
SELECT [TABLE_SCHEMA], [TABLE_NAME]
FROM [INFORMATION_SCHEMA].[TABLES]
WHERE [TABLE_TYPE] = 'BASE TABLE'
ORDER BY [TABLE_SCHEMA], [TABLE_NAME]";

			var listItems = new List<DbTableListItem>();

			using DbDataReader reader = await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);

			while (await reader.ReadAsync(cancellationToken: cancellationToken))
			{
				string ownerName = reader.GetString(0);
				string tableName = reader.GetString(1);

				var listItem = new DbTableListItem(tableName, ownerName);

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

			string qualifiedTableName = ownerName is null
				? $"[{tableName}]"
				: $"[{ownerName}].[{tableName}]";

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

			command.CommandTimeout = 10000;

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
			var connectionString = new SqlConnectionStringBuilder();

			if (!string.IsNullOrEmpty(ServerName))
			{
				connectionString.DataSource = ServerName;
			}

			if (!string.IsNullOrEmpty(UserName))
			{
				connectionString.UserID = UserName;

				if (!string.IsNullOrEmpty(Password))
				{
					connectionString.Password = Password;
				}
			}
			else
			{
				connectionString.IntegratedSecurity = true;
			}

			if (!string.IsNullOrWhiteSpace(DatabaseName))
			{
				connectionString.InitialCatalog = DatabaseName;
			}

			connectionString.ConnectTimeout = 5;

			return connectionString.ConnectionString;
		}
	}
}
#endif
