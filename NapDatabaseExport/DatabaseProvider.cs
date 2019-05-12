using System.Data;

namespace NapDatabaseExport
{
    public abstract class DatabaseProvider
    {
        public abstract string DatabaseTypeName { get; }

        public virtual bool UsesDatabaseFile
        {
            get { return false; }
        }

        public string DatabaseFile { get; set; }

        public virtual bool UsesServer
        {
            get { return false; }
        }

        public string Server { get; set; }

        public virtual bool UsesPort
        {
            get { return false; }
        }

        public virtual uint DefaultPort
        {
            get { return 0; }
        }

        public uint Port { get; set; }

        public virtual bool UsesUser
        {
            get { return false; }
        }

        public string User { get; set; }

        public virtual bool UsesPassword
        {
            get { return false; }
        }

        public string Password { get; set; }

        public virtual bool UsesDatabase
        {
            get { return false; }
        }

        public string Database { get; set; }

        public abstract bool TryConnect ();

        public abstract string [] GetDatabases ();

        public abstract string [] GetTables ();

        public abstract IDataReader ExecuteReader (string commandText, params DbParam [] parameters);

        public static DatabaseProvider [] GetAll ()
        {
            return new DatabaseProvider []
                {
                    new MySQLServerProvider (),
                    new SQLiteProvider (),
                    new MSSQLServerProvider (),
                    new MSAccessProvider (),
                    new ODBCProvider ()
                };
        }
    }
}
