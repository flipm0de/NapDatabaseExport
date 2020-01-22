using System;
using System.Runtime.Serialization;

namespace NraDatabaseExport.DbProviders
{
	/// <summary>
	/// Represents an error that occurred in a database provider.
	/// </summary>
	[Serializable]
	public class DbProviderException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DbProviderException"/> class.
		/// </summary>
		public DbProviderException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbProviderException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">the error message that explains the reason for the exception</param>
		public DbProviderException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbProviderException"/> class with a specified error message and a reference to the inner
		/// exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">the error message that explains the reason for the exception</param>
		/// <param name="innerException">the exception that is the cause of the current exception, or a null reference if no inner exception is
		/// specified.</param>
		public DbProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="DbProviderException"/> class with serialized data.
		/// </summary>
		/// <param name="info">the <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown</param>
		/// <param name="context">the <see cref="StreamingContext"/> that contains contextual information about the source or destination</param>
		protected DbProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
