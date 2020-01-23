#if SUPPORT_FIREBIRD
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using FirebirdSql.Data.FirebirdClient;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents a database provider for a Firebird Server.
	/// </summary>
	public class FirebirdDbProvider : DbProviderBase
	{
		private IDbConnection _connection;
		private IDbTransaction _transaction;

		#region DbProviderBase Members

		/// <inheritdoc/>
		public override string Name
			=> "Firebird";

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> true;

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesPort
			=> true;

		/// <inheritdoc/>
		public override int DefaultPort
			=> 3050;

		/// <inheritdoc/>
		public override bool UsesUserName
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override void CreateConnection()
		{
			string connectionString = CreateConnectionString();

			FbConnection connection;
			FbTransaction transaction;

			try
			{
				connection = new FbConnection(connectionString);

				connection.Open();

				transaction = connection.BeginTransaction();
			}
			catch (Exception ex)
			{
				throw new DbProviderException("Could not connect to the database.", ex);
			}

			_connection = connection;
			_transaction = transaction;
		}

		/// <inheritdoc/>
		public override string[] GetDatabaseNames()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public override string[] GetTableNames()
		{
			const string commandText = "select rdb$relation_name from rdb$relations where rdb$view_blr is null and(rdb$system_flag is null or rdb$system_flag = 0); ";

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
		public override IDataReader ExecuteTableReader(string tableName)
		{
			if (tableName is null)
			{
				throw new ArgumentNullException(nameof(tableName));
			}

			string commandText = $"SELECT * FROM \"{tableName}\"";

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
			command.Transaction = _transaction;

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

			_transaction = null;
			_connection = null;

			base.Dispose(disposing);
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
				Port = Port,
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
