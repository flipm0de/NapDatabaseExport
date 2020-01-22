#if SUPPORT_MSACCESS
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a Microsoft Access.
	/// </summary>
	public class MSAccessDbProvider : DbProviderBase
	{
		private OdbcConnection _connection;

		#region DbProviderBase Members

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
		public override void CreateConnection()
		{
			string connectionString = CreateConnectionString();

			OdbcConnection connection;

			try
			{
				connection = new OdbcConnection(connectionString);

				connection.Open();
			}
			catch (OdbcException ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			_connection = connection;
		}

		/// <inheritdoc/>
		public override string[] GetDatabaseNames()
			=> Array.Empty<string>();

		/// <inheritdoc/>
		public override string[] GetTableNames()
		{
			var tables = new List<string>();

			using (DataTable dataTable = _connection.GetSchema("Tables"))
			{
				foreach (DataRow dataRow in dataTable.Select("TABLE_TYPE='TABLE'"))
				{
					tables.Add(dataRow["TABLE_NAME"].ToString());
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
