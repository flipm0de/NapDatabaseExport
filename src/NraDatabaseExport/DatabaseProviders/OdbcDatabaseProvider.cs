#if SUPPORT_ODBC
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Text;

namespace NraDatabaseExport.DatabaseProviders
{
	public class OdbcDatabaseProvider : DatabaseProviderBase
	{
		private const int SQL_FETCH_NEXT = 1;
		private const int SQL_FETCH_FIRST_SYSTEM = 32;
		private const string SELECT_DSN_LABEL = "<select DSN>";

		#region DatabaseProviderBase Members

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
		public override bool UsesDatabase
			=> true;

		/// <inheritdoc/>
		public override bool UsesUser
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool TryConnect()
			=> true;

		/// <inheritdoc/>
		public override string[] GetDatabases()
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
		public override string[] GetTables()
		{
			if (Database == SELECT_DSN_LABEL)
			{
				return Array.Empty<string>();
			}

			var connection = CreateConnection();

			var tables = new List<string>();

			using (DataTable dataTable = connection.GetSchema("Tables"))
			{
				foreach (DataRow dataRow in dataTable.Select("TABLE_TYPE='TABLE'"))
				{
					tables.Add(dataRow["TABLE_NAME"].ToString());
				}
			}

			return tables.ToArray();
		}

		/// <inheritdoc/>
		public override IDataReader ExecuteReader(string commandText, params DatabaseParameter[] parameters)
		{
			IDataReader reader;

			using (OdbcCommand command = CreateCommand(commandText, parameters))
			{
				reader = command.ExecuteReader();

				// detach the SqlParameters from the command object, so they can be used again.
				command.Parameters.Clear();
			}

			return reader;
		}

		#endregion

		private string CreateConnectionString()
		{
			if (Database == SELECT_DSN_LABEL)
			{
				return "";
			}

			var dsn = "";

			if (!string.IsNullOrEmpty(Database))
			{
				dsn = Database;
			}

			var builder = new OdbcConnectionStringBuilder
			{
				Dsn = dsn,
			};

			builder.Add("Uid", User);
			builder.Add("Pwd", Password);

			return builder.ConnectionString;
		}

		private OdbcConnection CreateConnection()
		{
			string connectionString = CreateConnectionString();

			if (string.IsNullOrEmpty(connectionString))
			{
				return null;
			}

			try
			{
				var connection = new OdbcConnection(connectionString);

				connection.Open();

				return connection;
			}
			catch (OdbcException ex)
			{
				throw new DatabaseProviderException("Could not connect to an ODBC database.", ex);
			}
		}

		private OdbcCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			OdbcConnection connection = CreateConnection();

			if (connection is null)
			{
				return null;
			}

			OdbcCommand command = connection.CreateCommand();

			command.CommandText = commandText;

			if (parameters != null)
			{
				foreach (DatabaseParameter p in parameters)
				{
					command.Parameters.Add(
						new OdbcParameter(p.ParameterName, p.Value)
						{
							Direction = p.Direction,
						}
					);
				}
			}

			return command;
		}

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
	}
}
#endif
