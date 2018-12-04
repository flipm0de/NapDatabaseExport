using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;

namespace NapDatabaseExport
{
    public class MSAccessProvider : DatabaseProvider
    {
        public override string DatabaseTypeName
        {
            get { return "Microsoft Access база данни"; }
        }

        public override bool UsesDatabaseFile
        {
            get { return true; }
        }

        public override bool UsesPassword
        {
            get { return true; }
        }

        private string GenerateConnectionString ()
        {
            var builder = new OdbcConnectionStringBuilder
                {
                    Dsn = "MS Access Database",
                };

            builder.Add ("Dbq", DatabaseFile);
            builder.Add ("Uid", "Admin");
            builder.Add ("Pwd", Password);

            return builder.ConnectionString;
        }

        private OdbcConnection GetConnection ()
        {
            try {
                var conn = new OdbcConnection (GenerateConnectionString ());
                conn.Open ();
                return conn;
            } catch (OdbcException ex) {
                throw new DbConnectionLostException (ex);
            }
        }

        private OdbcCommand GetCommand (string commandText, IEnumerable<DbParam> parameters)
        {
            var connection = GetConnection ();
            if (connection == null)
                return null;

            var command = new OdbcCommand
                {
                    Connection = connection,
                    CommandText = commandText,
                    CommandType = CommandType.Text
                };

            if (parameters != null)
                foreach (var p in parameters)
                    command.Parameters.Add (new OdbcParameter (p.ParameterName, p.Value) { Direction = p.Direction });

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

            using (var dataTable = GetConnection ().GetSchema ("Tables"))
                foreach (var dataRow in dataTable.Select ("TABLE_TYPE='TABLE'"))
                    tables.Add (dataRow ["TABLE_NAME"].ToString ());

            return tables.ToArray ();
        }

        public override IDataReader ExecuteReader (string commandText, params DbParam [] parameters)
        {
            IDataReader reader;
            using (var command = GetCommand (commandText, parameters)) {
                reader = command.ExecuteReader ();

                // detach the SqlParameters from the command object, so they can be used again.
                command.Parameters.Clear ();
            }

            return reader;
        }
    }
}