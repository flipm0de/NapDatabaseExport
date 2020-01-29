#if SUPPORT_ODBC
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for ODBC.
	/// </summary>
	[DbProvider(DbProviderType.Odbc, "ODBC",
		UserNameCapability = ProviderCapabilityFlags.Supported,
		PasswordCapability = ProviderCapabilityFlags.Supported,
		DatabaseCapability = ProviderCapabilityFlags.SupportedAndRequired)]
	public class OdbcDbProvider : DbProviderBase
	{
		private const int SQL_FETCH_NEXT = 1;
		private const int SQL_FETCH_FIRST_SYSTEM = 32;
		private const string DefaultDatabaseName = "…";

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
			catch (Exception ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			return connection;
		}

		/// <inheritdoc/>
		public override ValueTask<DbDatabaseListItem[]> ListDatabasesAsync(DbConnection connection, CancellationToken cancellationToken = default)
		{
			if (connection is null)
			{
				throw new ArgumentNullException(nameof(connection));
			}

			var names = new List<string>
			{
				DefaultDatabaseName,
			};

			var envHandle = 0;

			if (OdbcWrapper.SQLAllocEnv(ref envHandle) != -1)
			{
				var serverName = new StringBuilder(1024);
				var driverName = new StringBuilder(1024);
				int snLen = 0;
				int driverLen = 0;
				int ret = OdbcWrapper.SQLDataSources(
					envHandle,
					SQL_FETCH_FIRST_SYSTEM,
					serverName,
					serverName.Capacity,
					ref snLen,
					driverName,
					driverName.Capacity,
					ref driverLen);

				while (ret == 0)
				{
					names.Add(serverName.ToString());

					ret = OdbcWrapper.SQLDataSources(
						envHandle,
						SQL_FETCH_NEXT,
						serverName,
						serverName.Capacity,
						ref snLen,
						driverName,
						driverName.Capacity,
						ref driverLen);
				}
			}

			names.Remove(DefaultDatabaseName);

			DbDatabaseListItem[] listItems = names
				.Select(x => new DbDatabaseListItem(x))
				.ToArray();

			return new ValueTask<DbDatabaseListItem[]>(listItems);
		}

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
				foreach (DataRow dataRow in dataTable.Select("TABLE_TYPE = 'TABLE'"))
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
			var builder = new OdbcConnectionStringBuilder();

			if (!string.IsNullOrWhiteSpace(DatabaseName))
			{
				builder.Dsn = DatabaseName;
			}

			builder.Add("Uid", UserName);
			builder.Add("Pwd", Password);

			return builder.ConnectionString;
		}

		#region Nested Type: OdbcWrapper

		private static class OdbcWrapper
		{
			[DllImport("odbc32.dll", CharSet = CharSet.Unicode)]
			public static extern int SQLDataSources(
				int EnvHandle,
				int Direction,
				StringBuilder ServerName,
				int ServerNameBufferLenIn,
				ref int ServerNameBufferLenOut,
				StringBuilder Driver,
				int DriverBufferLenIn,
				ref int DriverBufferLenOut);

			[DllImport("odbc32.dll")]
			public static extern int SQLAllocEnv(ref int EnvHandle);
		}

		#endregion
	}
}
#endif
