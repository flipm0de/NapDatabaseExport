#if SUPPORT_SQLSERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for Microsoft SQL Server.
	/// </summary>
	public class SqlServerDbProvider : DbProviderBase
	{
		private IDbConnection _connection;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override string Name
			=> "Microsoft SQL Server";

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesUserName
			=> true;

		/// <inheritdoc/>
		public override bool RequiresUserName
			=> false;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool RequiresPassword
			=> false;

		/// <inheritdoc/>
		public override bool UsesDatabaseName
			=> true;

		/// <inheritdoc/>
		public override void CreateConnection()
		{
			string connectionString = CreateConnectionString();

			SqlConnection connection;

			try
			{
				connection = new SqlConnection(connectionString);

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
			const string commandText = "SELECT name FROM sys.databases WHERE database_id > 4 ORDER BY name";

			var tables = new List<string>();

			using (IDataReader reader = ExecuteReader(commandText))
			{
				while (reader.Read())
				{
					tables.Add(reader.GetString(0));
				}
			}

			return tables.ToArray();
		}

		/// <inheritdoc/>
		public override string[] GetTableNames()
		{
			string commandText = $"SELECT name FROM [{DatabaseName}].dbo.sysobjects WHERE xtype = 'U' ORDER BY name";

			var tables = new List<string>();

			using (IDataReader dr = ExecuteReader(commandText))
			{
				while (dr.Read())
				{
					tables.Add(dr.GetString(0));
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

			string commandText = $"SELECT * FROM [{tableName}]";

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

		/// <inheritdoc/>
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_connection?.Dispose();
			}

			_connection = null;

			base.Dispose(disposing);
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

			if (!string.IsNullOrEmpty(DatabaseName))
			{
				connectionString.InitialCatalog = DatabaseName;
			}

			connectionString.ConnectTimeout = 5;

			return connectionString.ConnectionString;
		}
	}
}
#endif
