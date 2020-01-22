#if SUPPORT_FIREBIRD
using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using FirebirdSql.Data.FirebirdClient;

namespace NraDatabaseExport.DatabaseProviders
{
	public class FirebirdDatabaseProvider : DatabaseProviderBase
	{
		private FbTransaction _transaction;
		private FbConnection _connection;

		#region DatabaseProviderBase Members

		/// <inheritdoc/>
		public override string DatabaseTypeName
			=> "Firebird";

		/// <inheritdoc/>
		public override bool UsesServer
			=> true;

		/// <inheritdoc/>
		public override bool UsesPort
			=> true;

		/// <inheritdoc/>
		public override uint DefaultPort
			=> 3050;

		/// <inheritdoc/>
		public override bool UsesUser
			=> true;

		/// <inheritdoc/>
		public override bool UsesPassword
			=> true;

		/// <inheritdoc/>
		public override bool UsesQuoteTableNames
			=> true;

		/// <inheritdoc/>
		public override bool UsesDatabaseFile
			=> true;

		/// <inheritdoc/>
		public override string[] GetDatabases()
		{
			throw new NotSupportedException();
		}

		/// <inheritdoc/>
		public override string[] GetTables()
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
		public override IDataReader ExecuteReader(string commandText, params DatabaseParameter[] parameters)
		{
			if (commandText is null)
			{
				throw new ArgumentNullException(nameof(commandText));
			}

			FbCommand command = CreateCommand(commandText, parameters);

			IDataReader reader = command.ExecuteReader();
			if (reader is null)
			{
				throw new DatabaseProviderException("Could not get a valid reader from the database.");
			}

			// detach the SqlParameters from the command object, so they can be used again.
			command.Parameters.Clear();

			return reader;
		}

		#endregion

		private string CreateConnectionString()
		{
			return $"User={User};Password={Password};Database={DatabaseFile};DataSource={Server};Port={Port};Dialect=3;Charset=NONE; Role=;Connection lifetime=15;";
		}

		public override bool TryConnect()
		{
			if (string.IsNullOrWhiteSpace(Server) || string.IsNullOrWhiteSpace(User))
			{
				throw new Exception("The connection parameters are not properly set.");
			}

			Connect();

			return true;
		}

		private void Connect()
		{
			try
			{
				_connection = new FbConnection(CreateConnectionString());
				_connection.Open();
				_transaction = _connection.BeginTransaction();
			}
			catch (SocketException ex)
			{
				throw new DatabaseProviderException("Could not connect to a Firebird database.", ex);
			}
			catch (Exception ex)
			{
				throw new DatabaseProviderException("Could not connect to a Firebird database.", ex);
			}
		}

		private FbCommand CreateCommand(string commandText, IEnumerable<DatabaseParameter> parameters)
		{
			if (_connection is null)
			{
				throw new ApplicationException($"Connection is not opened.");
			}

			FbCommand command = _connection.CreateCommand();

			command.CommandText = commandText;
			command.Transaction = _transaction;

			if (parameters != null)
			{
				// parameters not tested yet
				foreach (DatabaseParameter p in parameters)
				{
					command.Parameters.Add(p.ParameterName, p.Value);
				}
			}

			return command;
		}
	}
}
#endif
