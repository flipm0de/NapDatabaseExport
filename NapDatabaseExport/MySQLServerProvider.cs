using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using MySql.Data.MySqlClient;

namespace NapDatabaseExport
{
    public class MySQLServerProvider : DatabaseProvider
    {
        public override string DatabaseTypeName
        {
            get { return "MySQL Сървър"; }
        }

        public override bool UsesServer
        {
            get { return true; }
        }

        public override bool UsesPort
        {
            get { return true; }
        }

        public override uint DefaultPort
        {
            get { return 3306; }
        }

        public override bool UsesUser
        {
            get { return true; }
        }

        public override bool UsesPassword
        {
            get { return true; }
        }

        public override bool UsesDatabase
        {
            get { return true; }
        }

        private string GenerateConnectionString ()
        {
            var connectionString = new MySqlConnectionStringBuilder ();

            if (!string.IsNullOrEmpty (Server)) {
                connectionString.Server = Server;
                if (Port > 0)
                    connectionString.Port = Port;
            }

            if (!string.IsNullOrEmpty (User))
                connectionString.UserID = User;

            if (!string.IsNullOrEmpty (Password))
                connectionString.Password = Password;

            // MySQL Server 5.1 requires the database paramater to be passed
            connectionString.Database = string.IsNullOrEmpty (Database) ? "information_schema" : Database;

            connectionString.CharacterSet = "utf8";
            connectionString.AllowUserVariables = true;

            return connectionString.GetConnectionString (true);
        }

        private object GetConnection ()
        {
            try {
                var conn = new MySqlConnection (GenerateConnectionString ());
                conn.Open ();
                return conn;
            } catch (SocketException ex) {
                throw new DbConnectionLostException (ex);
            } catch (MySqlException ex) {
                throw new DbConnectionLostException (ex);
            }
        }

        private MySqlCommand GetCommand (string commandText, IEnumerable<DbParam> parameters)
        {
            var connection = (MySqlConnection) GetConnection ();
            if (connection == null)
                return null;

            var command = new MySqlCommand
                {
                    Connection = connection,
                    CommandText = commandText,
                    CommandType = CommandType.Text
                };

            if (parameters != null)
                foreach (var p in parameters)
                    command.Parameters.Add (new MySqlParameter (p.ParameterName, p.Value) { Direction = p.Direction });

            return command;
        }

        public override bool TryConnect ()
        {
            if (string.IsNullOrWhiteSpace (Server) || string.IsNullOrWhiteSpace (User))
                throw new Exception ("The connection parameters are not properly set.");

            MySqlConnection conn = null;
            try {
                conn = (MySqlConnection) GetConnection ();
            } finally {
                conn?.Close ();
            }

            return true;
        }

        public override void Disconnect ()
        {
            throw new NotImplementedException ();
        }

        public override string [] GetDatabases ()
        {
            var dbs = new List<string> ();

            using (var dr = ExecuteReader ("SHOW DATABASES"))
                while (dr.Read ())
                    dbs.Add (dr.GetString (0));

            dbs.Remove ("information_schema");
            dbs.Remove ("performance_schema");
            dbs.Remove ("mysql");

            return dbs.ToArray ();
        }

        public override string [] GetTables ()
        {
            var tables = new List<string> ();

            using (var dr = ExecuteReader (string.Format ("SHOW TABLES IN `{0}`", Database)))
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