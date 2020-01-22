#if SUPPORT_ODBC
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Text;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for ODBC.
	/// </summary>
	public class OdbcDbProvider : DbProviderBase
	{
		private const int SQL_FETCH_NEXT = 1;
		private const int SQL_FETCH_FIRST_SYSTEM = 32;
		private const string SELECT_DSN_LABEL = "<select DSN>";

		private OdbcConnection _connection;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> IntPtr.Size == 4
				? "ODBC32 източник на данни"
				: "ODBC64 източник на данни";

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> false;

		/// <inheritdoc/>
		public override bool UsesServer
			=> false;

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

			OdbcConnection connection;

			try
			{
				connection = new OdbcConnection(connectionString);

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
			var dbs = new List<string> { SELECT_DSN_LABEL };

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
					dbs.Add(serverName.ToString());

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

			return dbs.ToArray();
		}

		/// <inheritdoc/>
		public override string[] GetTableNames()
		{
			if (DatabaseName == SELECT_DSN_LABEL)
			{
				return Array.Empty<string>();
			}

			var tableNames = new List<string>();

			using (DataTable dataTable = _connection.GetSchema("Tables"))
			{
				foreach (DataRow dataRow in dataTable.Select("TABLE_TYPE='TABLE'"))
				{
					string tableName = dataRow["TABLE_NAME"].ToString();

					tableNames.Add(tableName);
				}
			}

			return tableNames.ToArray();
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
			base.Dispose(disposing);

			if (disposing)
			{
				_connection?.Dispose();
			}
		}

		#endregion

		private string CreateConnectionString()
		{
			if (DatabaseName == SELECT_DSN_LABEL)
			{
				return string.Empty;
			}

			string dsn = string.Empty;

			if (!string.IsNullOrEmpty(DatabaseName))
			{
				dsn = DatabaseName;
			}

			var builder = new OdbcConnectionStringBuilder
			{
				Dsn = dsn,
			};

			builder.Add("Uid", UserName);
			builder.Add("Pwd", Password);

			return builder.ConnectionString;
		}

		#region Nested Type: OdbcWrapper

		private static class OdbcWrapper
		{
			[DllImport("odbc32.dll")]
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
