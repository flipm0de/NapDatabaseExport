using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Net.Sockets;

namespace NapDatabaseExport
{
    public class MSSQLServerProvider : DatabaseProvider
    {
        public override string DatabaseTypeName
        {
            get { return "Microsoft SQL Сървър"; }
        }

        public override bool UsesServer
        {
            get { return true; }
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
            var connectionString = new SqlConnectionStringBuilder ();
            if (!string.IsNullOrEmpty (Server))
                connectionString.DataSource = Server;

            if (!string.IsNullOrEmpty (User))
                connectionString.UserID = User;

            if (!string.IsNullOrEmpty (Password))
                connectionString.Password = Password;

            if (!string.IsNullOrEmpty (Database))
                connectionString.InitialCatalog = Database;

            connectionString.ConnectTimeout = 5;

            return connectionString.ConnectionString;
        }

        private SqlConnection GetConnection ()
        {
            try {
                var conn = new SqlConnection (GenerateConnectionString ());
                conn.Open ();
                return conn;
            } catch (SocketException ex) {
                throw new DbConnectionLostException (ex);
            } catch (SqlException ex) {
                throw new DbConnectionLostException (ex);
            }
        }

        private SqlCommand GetCommand (string commandText, IEnumerable<DbParam> parameters)
        {
            var connection = GetConnection ();
            if (connection == null)
                return null;

            var command = new SqlCommand
                {
                    Connection = connection,
                    CommandText = commandText,
                    CommandType = CommandType.Text,
                    CommandTimeout = 10000
                };

            if (parameters != null)
                foreach (var p in parameters) {
                    var value = p.Value ?? DBNull.Value;
                    var sqlParameter = new SqlParameter (p.ParameterName, value) { Direction = p.Direction };
                    if (p.DataType != null)
                        sqlParameter.SqlDbType = p.DataType.Value;

                    command.Parameters.Add (sqlParameter);
                }

            return command;
        }

        public override bool TryConnect ()
        {
            if (string.IsNullOrWhiteSpace (Server) || string.IsNullOrWhiteSpace (User))
                throw new Exception ("The connection parameters are not properly set.");

            SqlConnection conn = null;
            try {
                conn = GetConnection ();
            } finally {
                conn?.Close ();
            }

            return true;
        }

        public override string [] GetDatabases ()
        {
            var tables = new List<string> ();

            using (var dr = ExecuteReader ("SELECT name FROM sys.databases"))
                while (dr.Read ())
                    tables.Add (dr.GetString (0));

            return tables.ToArray ();
        }

        public override string [] GetTables ()
        {
            var tables = new List<string> ();

            using (var dr = ExecuteReader (string.Format ("select name from [{0}].[sys].sysobjects where xtype = 'U'", Database)))
                while (dr.Read ())
                    tables.Add (dr.GetString (0));

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