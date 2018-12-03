using System;

namespace NapDatabaseExport
{
    public class DbConnectionLostException : SystemException
    {
        public DbConnectionLostException (string message)
            : base (message)
        {
        }

        public DbConnectionLostException (Exception inner)
            : base ("The connection with database was interrupted!", inner)
        {
        }

        public DbConnectionLostException (string message, Exception inner)
            : base (message, inner)
        {
        }
    }
}