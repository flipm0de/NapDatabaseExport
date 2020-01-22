#if SUPPORT_SQLSERVER
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;

namespace NraDatabaseExport.DatabaseProviders
{
	public class SqlServerDatabaseProvider : DatabaseProviderBase
	{
		#region DatabaseProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> "Microsoft SQL Server";

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesUser
			=> true;

		/// <inheritdoc/>
		public override bool RequiresUser
			=> false;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool RequiresPassword
			=> false;

		/// <inheritdoc/>
		public override bool UsesDatabase
			=> true;

		/// <inheritdoc/>
		public override bool TryConnect()
		{
			if (string.IsNullOrWhiteSpace(Server))
			{
				throw new Exception("The connection parameters are not properly set.");
			}

			SqlConnection connection = null;

			try
			{
				connection = CreateConnection();
			}
			finally
			{
				connection?.Close();
			}

			return true;
		}

		/// <inheritdoc/>
		public override string[] GetDatabases()
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
		public override string[] GetTables()
		{
			string commandText = $"SELECT name FROM [{Database}].dbo.sysobjects WHERE xtype = 'U' ORDER BY name";

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
		public override IDataReader ExecuteReader(string commandText, params DatabaseParameter[] parameters)
		{
			IDataReader reader;

			using (SqlCommand command = CreateCommand(commandText, parameters))
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
			var connectionString = new SqlConnectionStringBuilder();

			if (!string.IsNullOrEmpty(Server))
			{
				connectionString.DataSource = Server;
			}

			if (!string.IsNullOrEmpty(User))
			{
				connectionString.UserID = User;

				if (!string.IsNullOrEmpty(Password))
				{
					connectionString.Password = Password;
				}
			}
			else
			{
				connectionString.IntegratedSecurity = true;
			}

			if (!string.IsNullOrEmpty(Database))
			{
				connectionString.InitialCatalog = Database;
			}

			connectionString.ConnectTimeout = 5;

			return connectionString.ConnectionString;
		}

		private SqlConnection CreateConnection()
		{
			try
			{
				var connection = new SqlConnection(CreateConnectionString());

				connection.Open();

				return connection;
			}
			catch (SocketException ex)
			{
				throw new DatabaseProviderException("Could not connect to a Microsoft SQL Server database.", ex);
			}
			catch (SqlException ex)
			{
				throw new DatabaseProviderException("Could not connect to a Microsoft SQL Server database.", ex);
			}
		}

		private SqlCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			SqlConnection connection = CreateConnection();

			if (connection is null)
			{
				return null;
			}

			SqlCommand command = connection.CreateCommand();

			command.CommandText = commandText;
			command.CommandTimeout = 10000;

			if (parameters != null)
			{
				foreach (DatabaseParameter p in parameters)
				{
					var value = p.Value ?? DBNull.Value;

					var sqlParameter = new SqlParameter(p.ParameterName, value)
					{
						Direction = p.Direction,
					};

					if (p.DataType != null)
					{
						sqlParameter.SqlDbType = p.DataType.Value;
					}

					command.Parameters.Add(sqlParameter);
				}
			}

			return command;
		}
	}
}
#endif
