using System.Collections.Generic;
using System.Data;
using Mono.Data.Sqlite;

namespace NapDatabaseExport
{
    public class SQLiteProvider : DatabaseProvider
    {
        private const int BUSY_TIMEOUT = 10 * 1000;

        public override string DatabaseTypeName
        {
            get { return "SQLite база данни"; }
        }

        public override bool UsesDatabaseFile
        {
            get { return true; }
        }

        private string GenerateConnectionString ()
        {
            var builder = new SqliteConnectionStringBuilder
                {
                    DataSource = DatabaseFile,
                    //Pooling = true,
                    Version = 3,
                    DefaultTimeout = BUSY_TIMEOUT
                };

            return builder.ConnectionString;
        }

        private SqliteConnection GetConnection ()
        {
            try {
                var conn = new SqliteConnection (GenerateConnectionString ());
                conn.Open ();
                return conn;
            } catch (SqliteException ex) {
                throw new DbConnectionLostException (ex);
            }
        }

        private SqliteCommand GetCommand (string commandText, IEnumerable<DbParam> parameters)
        {
            var connection = GetConnection ();
            if (connection == null)
                return null;

            var command = new SqliteCommand
                {
                    Connection = connection,
                    CommandText = commandText,
                    CommandType = CommandType.Text
                };

            if (parameters != null)
                foreach (var p in parameters)
                    command.Parameters.Add (new SqliteParameter (p.ParameterName, p.Value) { Direction = p.Direction });

            return command;
        }

        public override bool TryConnect ()
        {
            return true;
        }

        public override string [] GetDatabases ()
        {
            return new string [0];
        }

        public override string [] GetTables ()
        {
            var tables = new List<string> ();

            using (var dr = ExecuteReader ("SELECT name FROM sqlite_master WHERE type='table'"))
                while (dr.Read ())
                    tables.Add (dr.GetString (0));

            return tables.ToArray ();
        }

        public override IDataReader ExecuteReader (string commandText, params DbParam [] parameters)
        {
            IDataReader reader;
            using (var command = GetCommand (commandText, parameters)) {
                reader = command.ExecuteReader ();
                if (reader == null)
                    throw new DbConnectionLostException ("Could not get a valid reader from the database");

                // detach the SqlParameters from the command object, so they can be used again.
                command.Parameters.Clear ();
            }

            return reader;
        }
    }
}