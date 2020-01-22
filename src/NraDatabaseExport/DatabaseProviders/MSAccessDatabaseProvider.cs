#if SUPPORT_MSACCESS
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace NraDatabaseExport.DatabaseProviders
{
	public class MSAccessDatabaseProvider : DatabaseProviderBase
	{
		#region DatabaseProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> "Microsoft Access";

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool TryConnect()
			=> true;

		/// <inheritdoc/>
		public override string[] GetDatabases()
			=> Array.Empty<string>();

		/// <inheritdoc/>
		public override string[] GetTables()
		{
			var tables = new List<string>();

			using (var dataTable = CreateConnection().GetSchema("Tables"))
			{
				foreach (var dataRow in dataTable.Select("TABLE_TYPE='TABLE'"))
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
			using (var command = CreateCommand(commandText, parameters))
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
			var builder = new OdbcConnectionStringBuilder
			{
				Dsn = "MS Access Database",
			};

			builder.Add("Dbq", DatabaseFile);
			builder.Add("Uid", "Admin");
			builder.Add("Pwd", Password);

			return builder.ConnectionString;
		}

		private OdbcConnection CreateConnection()
		{
			try
			{
				OdbcConnection connection = new OdbcConnection(CreateConnectionString());

				connection.Open();

				return connection;
			}
			catch (OdbcException ex)
			{
				throw new DatabaseProviderException("Could not connect to a Microsoft Access database.", ex);
			}
		}

		private OdbcCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			OdbcConnection connection = CreateConnection();

			if (connection is null)
			{
				throw new ApplicationException($"Connection is not opened.");
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
						});
				}
			}

			return command;
		}
	}
}
#endif
