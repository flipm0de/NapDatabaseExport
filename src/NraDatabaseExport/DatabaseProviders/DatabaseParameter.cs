using System.Data;

namespace NraDatabaseExport.DatabaseProviders
{
	public class DatabaseParameter
	{
		public string ParameterName { get; set; }

		public object Value { get; set; }

		public ParameterDirection Direction { get; set; } = ParameterDirection.Input;

		public SqlDbType? DataType { get; set; }

		public DatabaseParameter(string parameterName, object value)
		{
			ParameterName = parameterName;
			Value = value;
		}

		public override string ToString()
			=> $"{ParameterName} = {Value}";
	}
}
