using System;
using System.Runtime.Serialization;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an error that occurred in an export provider.
	/// </summary>
	[Serializable]
	public class ExportProviderException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ExportProviderException"/> class.
		/// </summary>
		public ExportProviderException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportProviderException"/> class with a specified error message.
		/// </summary>
		/// <param name="message">the error message that explains the reason for the exception</param>
		public ExportProviderException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportProviderException"/> class with a specified error message and a reference to the inner
		/// exception that is the cause of this exception.
		/// </summary>
		/// <param name="message">the error message that explains the reason for the exception</param>
		/// <param name="innerException">the exception that is the cause of the current exception, or a null reference if no inner exception is
		/// specified.</param>
		public ExportProviderException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="ExportProviderException"/> class with serialized data.
		/// </summary>
		/// <param name="info">the <see cref="SerializationInfo"/> that holds the serialized object data about the exception being thrown</param>
		/// <param name="context">the <see cref="StreamingContext"/> that contains contextual information about the source or destination</param>
		protected ExportProviderException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
