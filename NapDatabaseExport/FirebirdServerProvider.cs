using System;
using System.Collections.Generic;
using System.Data;
using System.Net.Sockets;
using FirebirdSql.Data.FirebirdClient;

namespace NapDatabaseExport
{
    public class FirebirdServerProvider : DatabaseProvider
    {
        public override string DatabaseTypeName => "Firebird Сървър";

        public override bool UsesServer => true;

        public override bool UsesPort => true;

        public override uint DefaultPort => 3050;

        public override bool UsesUser => true;

        public override bool UsesPassword => true;
        public override bool UsesQuoteTableNames => true;

        public override bool UsesDatabaseFile=>true;
        private FbTransaction _transaction;
        private FbConnection _connection;
        private string GenerateConnectionString ()
        {

            return 
                $"User={User};Password={Password};Database={DatabaseFile};DataSource={Server};Port={Port};Dialect=3;Charset=NONE; Role=;Connection lifetime=15;";

        }

        private void Connect ()
        {
            try {
                _connection = new FbConnection(GenerateConnectionString ());
                _connection.Open ();
                _transaction = _connection.BeginTransaction();
            } catch (SocketException ex) {
                throw new DbConnectionLostException (ex);
            } catch (Exception ex) {
                throw new DbConnectionLostException (ex);
            }
        }

        private FbCommand GetCommand (string commandText, IEnumerable<DbParam> parameters)
        {

            if (_connection == null)
                return null;

            var command = new FbCommand
            {
                Connection = _connection,
                CommandText = commandText,
                CommandType = CommandType.Text,
                Transaction = _transaction
            };
            if (parameters != null)
                foreach (var p in parameters) // parameters not tested yet 
                    command.Parameters.Add(p.ParameterName, p.Value);

            return command;
        }

        public override bool TryConnect ()
        {
            if (string.IsNullOrWhiteSpace (Server) || string.IsNullOrWhiteSpace (User))
                throw new Exception ("The connection parameters are not properly set.");

            Connect ();

            return true;
        }

        public override string [] GetDatabases ()
        {
            throw new NotSupportedException();
        }

        public override string [] GetTables ()
        {
            var tables = new List<string> ();

            using (var dr =
                ExecuteReader(
                    "select rdb$relation_name from rdb$relations where rdb$view_blr is null and(rdb$system_flag is null or rdb$system_flag = 0); ")
            )
            {
                while (dr.Read())
                    tables.Add(dr.GetString(0));

            }

            return tables.ToArray ();
        }

        public override IDataReader ExecuteReader(string commandText, params DbParam[] parameters)
        {
            var command = GetCommand(commandText, parameters);
            IDataReader reader = command.ExecuteReader();
            if (reader == null)
                throw new DbConnectionLostException("Could not get a valid reader from the database");

            // detach the SqlParameters from the command object, so they can be used again.
            command.Parameters.Clear();


            return reader;
        }
    }
}