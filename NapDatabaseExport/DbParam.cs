using System.Data;

namespace NapDatabaseExport
{
    public class DbParam
    {
        private string parameterName;
        public string ParameterName
        {
            get { return parameterName; }
            set { parameterName = value; }
        }

        private object value;
        public object Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private ParameterDirection direction = ParameterDirection.Input;
        public ParameterDirection Direction
        {
            get { return direction; }
            set { direction = value; }
        }

        public SqlDbType? DataType { get; set; }

        public DbParam (string parameterName, object value)
        {
            this.parameterName = parameterName;
            this.value = value;
        }

        public override string ToString ()
        {
            return string.Format ("{0} = {1}", parameterName, value);
        }
    }
}