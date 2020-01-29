#if SUPPORT_SQLITE
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for SQLite.
	/// </summary>
	[DbProvider(DbProviderType.Sqlite, "SQLite",
		FileCapability = ProviderCapabilityFlags.SupportedAndRequired)]
	public class SqliteDbProvider : DbProviderBase
	{
		//private const int BUSY_TIMEOUT = 10 * 1000;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
		{
			string connectionString = CreateConnectionString();

			SqliteConnection connection;

			try
			{
				connection = new SqliteConnection(connectionString);

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

			const string commandText = @"SELECT ""name"" FROM ""sqlite_master"" WHERE ""type"" = 'table' ORDER BY ""name""";

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

			string commandText = $"SELECT * FROM {tableName}";

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
