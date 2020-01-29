#if SUPPORT_MSACCESS
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Threading;
using System.Threading.Tasks;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a Microsoft Access.
	/// </summary>
	[DbProvider(DbProviderType.MSAccess, "Microsoft Access",
		FileCapability = ProviderCapabilityFlags.SupportedAndRequired,
		PasswordCapability = ProviderCapabilityFlags.Supported)]
	public class MSAccessDbProvider : DbProviderBase
	{
		#region DbProviderBase Members

		/// <inheritdoc/>
		public override async ValueTask<DbConnection> OpenConnectionAsync(CancellationToken cancellationToken = default)
		{
			string connectionString = CreateConnectionString();

			OdbcConnection connection;

			try
			{
				connection = new OdbcConnection(connectionString);

				await connection.OpenAsync(cancellationToken);
			}
			catch (OdbcException ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			return connection;
		}

		/// <inheritdoc/>
		public override ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(DbConnection connection, CancellationToken cancellationToken = default)
			=> new ValueTask<DbDatabaseListItem[]>(Array.Empty<DbDatabaseListItem>());

		/// <inheritdoc/>
		public override ValueTask<DbTableListItem[]> ListTablesAsync(DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			if (!(connection is OdbcConnection odbcConnection))
			{
				throw new ArgumentException($"An ODBC connection expected but {connection.GetType()} was provided instead.", nameof(connection));
			}

			var listItems = new List<DbTableListItem>();

			using (DataTable dataTable = odbcConnection.GetSchema("Tables"))
			{
				foreach (DataRow dataRow in dataTable.Select("TABLE_TYPE='TABLE'"))
				{
					string tableName = dataRow["TABLE_NAME"].ToString();

					var listItem = new DbTableListItem(tableName);

					listItems.Add(listItem);
				}
			}

			return new ValueTask<DbTableListItem[]>(listItems.ToArray());
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

			var builder = new OdbcConnectionStringBuilder
			{
				Dsn = "MS Access Database",
			};

			builder.Add("Dbq", DatabaseFileName);
			builder.Add("Uid", "Admin");
			builder.Add("Pwd", Password);

			string connectionString = builder.ConnectionString;

			return connectionString;
		}
	}
}
#endif
