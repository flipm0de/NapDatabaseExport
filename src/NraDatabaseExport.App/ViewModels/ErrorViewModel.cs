using System;
using NraDatabaseExport.App.Infrastructure;

namespace NraDatabaseExport.App.ViewModels
{
	/// <summary>
	/// Represents a view model for an error.
	/// </summary>
	public class ErrorViewModel : ViewModelBase
	{
		/// <summary>
		/// Gets the exception that is the cause of the error.
		/// </summary>
		public Exception Exception { get; }

		/// <summary>
		/// Gets the title of the error.
		/// </summary>
		public string Title { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="ErrorViewModel"/> class with a specified exception that caused the error.
		/// </summary>
		/// <param name="exception">the exception that is the cause of the error</param>
		/// <param name="title">the title of the error</param>
		public ErrorViewModel(Exception exception, string title = null)
		{
			Exception = exception ?? throw new ArgumentNullException(nameof(exception));
			Title = title;
		}
	}
}
