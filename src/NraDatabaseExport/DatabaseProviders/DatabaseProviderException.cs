using System;
using System.Runtime.Serialization;

namespace NraDatabaseExport.DatabaseProviders
{
	/// <summary>
	/// Represents an error that occurred in a database provider.
	/// </summary>
	[Serializable]
	public class DatabaseProviderException : Exception
	{
		public DatabaseProviderException()
		{
		}

		public DatabaseProviderException(string message)
			: base(message)
		{
		}

		public DatabaseProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		protected DatabaseProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
