#if SUPPORT_FIREBIRD
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using FirebirdSql.Data.FirebirdClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a Firebird Server.
	/// </summary>
	[DbProvider(DbProviderType.Firebird, "Firebird Server",
		FileCapability = ProviderCapabilityFlags.Supported,
		ServerCapability = ProviderCapabilityFlags.Supported,
		PortCapability = ProviderCapabilityFlags.Supported,
		PasswordCapability = ProviderCapabilityFlags.Supported)]
	public class FirebirdDbProvider : DbProviderBase
	{
		#region DbProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
		{
			string connectionString = CreateConnectionString();

			FbConnection connection;

			try
			{
				connection = new FbConnection(connectionString);

				await connection.OpenAsync(cancellationToken);
			}
			catch (Exception ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			return connection;
		}

		/// <inheritdoc/>
		public override ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(DbConnection connection, CancellationToken cancellationToken = default)
			=> new ValueTask<DbDatabaseListItem[]>(Array.Empty<DbDatabaseListItem>());

		/// <inheritdoc/>
		public override async ValueTask<DbTableListItem[]> ListTablesAsync(DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			const string commandText = @"
select rdb$relation_name
from rdb$relations
where rdb$view_blr is null
	and (rdb$system_flag is null or rdb$system_flag = 0);";

			var listItems = new List<DbTableListItem>();

			using DbDataReader reader = await ExecuteReaderAsync(connection, commandText, cancellationToken: cancellationToken);

			while (await reader.ReadAsync(cancellationToken: cancellationToken))
			{
				string tableName = reader.GetString(0);

				var listItem = new DbTableListItem(tableName);

				listItems.Add(listItem);
			}

			return listItems.ToArray();
		}

		/// <inheritdoc/>
		public override async ValueTask<DbDataReader> ExecuteTableReaderAsync(DbConnection connection, string tableName, string? ownerName, CancellationToken cancellationToken = default)
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

			string commandText = $"SELECT * FROM \"{tableName}\"";

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

			if (string.IsNullOrWhiteSpace(DatabaseFileName) && string.IsNullOrWhiteSpace(ServerName))
			{
				throw new Exception("Neither database file name nor server name is specified.");
			}

			var builder = new FbConnectionStringBuilder
			{
				UserID = UserName,
				Password = Password,
				Database = DatabaseFileName,
				DataSource = ServerName,
				Port = Port ?? 3050,
				Dialect = 3,
				Charset = "NONE",
				Role = string.Empty,
				ConnectionLifeTime = 5,
			};

			string connectionString = builder.ConnectionString;

			return connectionString;
		}
	}
}
#endif
