using System.Data;

namespace NraDatabaseExport.DatabaseProviders
{
	public abstract class DatabaseProviderBase
	{
		public abstract string DatabaseTypeName { get; }

		public virtual bool UsesDatabaseFile
			=> false;

		public string DatabaseFile { get; set; }

		public virtual bool UsesServer
			=> false;

		public string Server { get; set; }

		public virtual bool UsesPort
			=> false;

		public virtual uint DefaultPort
			=> 0;

		public uint Port { get; set; }

		public virtual bool UsesUser
			=> false;

		public virtual bool RequiresUser
			=> false;

		public string User { get; set; }

		public virtual bool UsesPassword
			=> false;

		public virtual bool RequiresPassword
			=> false;

		public string Password { get; set; }

		public virtual bool UsesDatabase
			=> false;

		public virtual bool UsesQuoteTableNames
			=> false;

		public string Database { get; set; }

		public abstract bool TryConnect();

		public abstract string[] GetDatabases();

		public abstract string[] GetTables();

		public abstract IDataReader ExecuteReader(string commandText, params DatabaseParameter[] parameters);
	}
}
