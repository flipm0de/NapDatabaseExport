using System;
using System.Collections.Generic;
using System.Data;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Provides a base type for database providers.
	/// </summary>
	public abstract class DbProviderBase : IDbProvider
	{
		#region IDbProvider Members

		/// <inheritdoc/>
		public abstract string DatabaseTypeName { get; }

		/// <inheritdoc/>
		public virtual bool UsesDatabaseFile
			=> false;

		/// <inheritdoc/>
		public string DatabaseFileName { get; set; }

		/// <inheritdoc/>
		public virtual bool UsesServer
			=> false;

		/// <inheritdoc/>
		public string ServerName { get; set; }

		/// <inheritdoc/>
		public virtual bool UsesPort
			=> false;

		/// <inheritdoc/>
		public virtual int DefaultPort
			=> 0;

		/// <inheritdoc/>
		public int Port { get; set; }

		/// <inheritdoc/>
		public virtual bool UsesUserName
			=> false;

		/// <inheritdoc/>
		public virtual bool RequiresUserName
			=> false;

		/// <inheritdoc/>
		public string UserName { get; set; }

		/// <inheritdoc/>
		public virtual bool UsesPassword
			=> false;

		/// <inheritdoc/>
		public virtual bool RequiresPassword
			=> false;

		/// <inheritdoc/>
		public string Password { get; set; }

		/// <inheritdoc/>
		public virtual bool UsesDatabaseName
			=> false;

		/// <inheritdoc/>
		public string DatabaseName { get; set; }

		/// <inheritdoc/>
		public abstract void CreateConnection();

		/// <inheritdoc/>
		public abstract string[] GetDatabaseNames();

		/// <inheritdoc/>
		public abstract string[] GetTableNames();

		/// <inheritdoc/>
		public abstract IDataReader ExecuteTableReader(string tableName);

		#endregion

		#region IDisposable Members

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Disposes any unmanaged resources allocated by this object.
		/// </summary>
		~DbProviderBase()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases any resources allocated by this object.
		/// </summary>
		/// <param name="disposing">the flag indicating whether the object is being disposed, or the garbage collector has kicked in</param>
		protected virtual void Dispose(bool disposing)
		{
		}

		/// <summary>
		/// Executes a reader for the result of a command with a specified text.
		/// </summary>
		/// <param name="commandText">the text of the command to execute</param>
		/// <param name="parameters">the database parameters to associate with the command</param>
		/// <returns>the data reader containing the result of the command execution</returns>
		protected IDataReader ExecuteReader(string commandText, params IDbDataParameter[] parameters)
		{
			if (commandText is null)
			{
				throw new ArgumentNullException(nameof(commandText));
			}

			IDbCommand command = CreateCommand(commandText, parameters);

			IDataReader reader = command.ExecuteReader();

			if (reader is null)
			{
				throw new DbProviderException("Could not execute a data reader on the database.");
			}

			// detach the parameters from the command object, so they can be used again.
			command.Parameters.Clear();

			return reader;
		}

		/// <summary>
		/// Creates a database command with the specified text.
		/// </summary>
		/// <param name="commandText">the text of the command to create</param>
		/// <param name="parameters">the list of database parameters to add to the command</param>
		/// <returns>the created database command</returns>
		protected abstract IDbCommand CreateCommand(string commandText, IEnumerable<IDbDataParameter> parameters = null);
	}
}
