using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Runtime.InteropServices;
using System.Text;

namespace NapDatabaseExport
{
    public static class OdbcWrapper
    {
        [DllImport ("odbc32.dll")]
        public static extern int SQLDataSources (int EnvHandle, int Direction, StringBuilder ServerName, int ServerNameBufferLenIn,
            ref int ServerNameBufferLenOut, StringBuilder Driver, int DriverBufferLenIn, ref int DriverBufferLenOut);

        [DllImport ("odbc32.dll")]
        public static extern int SQLAllocEnv (ref int EnvHandle);
    }

    public class ODBCProvider : DatabaseProvider
    {
        private const int SQL_FETCH_NEXT = 1;
        private const int SQL_FETCH_FIRST_SYSTEM = 32;
        private const string SELECT_DSN_LABEL = "<select DSN>";

        public override string DatabaseTypeName
        {
            get
            {
                return IntPtr.Size == 4 ?
                    "ODBC32 източник на данни" :
                    "ODBC64 източник на данни";
            }
        }

        public override bool UsesDatabaseFile
        {
            get { return false; }
        }

        public override bool UsesServer
        {
            get { return false; }
        }

        public override bool UsesDatabase
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

        private string GenerateConnectionString ()
        {
            if (Database == SELECT_DSN_LABEL)
                return "";

            var dsn = "";

            if (!string.IsNullOrEmpty (Database))
                dsn = Database;

            var builder = new OdbcConnectionStringBuilder
                {
                    Dsn = dsn,
                };

            builder.Add ("Uid", User);
            builder.Add ("Pwd", Password);

            return builder.ConnectionString;
        }

        private OdbcConnection GetConnection ()
        {
            var cs = GenerateConnectionString ();
            if (cs == "")
                return null;

            try {
                var conn = new OdbcConnection (cs);
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
            var dbs = new List<string> { SELECT_DSN_LABEL };

            var envHandle = 0;

            if (OdbcWrapper.SQLAllocEnv (ref envHandle) != -1) {
                var serverName = new StringBuilder (1024);
                var driverName = new StringBuilder (1024);
                var snLen = 0;
                var driverLen = 0;
                var ret = OdbcWrapper.SQLDataSources (envHandle, SQL_FETCH_FIRST_SYSTEM, serverName, serverName.Capacity, ref snLen,
                    driverName, driverName.Capacity, ref driverLen);
                while (ret == 0) {
                    dbs.Add (serverName.ToString ());
                    ret = OdbcWrapper.SQLDataSources (envHandle, SQL_FETCH_NEXT, serverName, serverName.Capacity, ref snLen,
                        driverName, driverName.Capacity, ref driverLen);
                }
            }

            return dbs.ToArray ();
        }

        public override string [] GetTables ()
        {
            if (Database == SELECT_DSN_LABEL)
                return new string [0];

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