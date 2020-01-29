using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Provides a mechanism for exporting a data table to a file.
	/// </summary>
	public interface IExportProvider : IDisposable
	{
		/// <summary>
		/// Gets or sets the culture to use when exporting values.
		/// </summary>
		public CultureInfo Culture { get; set; }

		/// <summary>
		/// Opens a file with a specified name to writing data into.
		/// </summary>
		/// <param name="fileName">the name of the file to export data to</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of opening the file to write</returns>
		ValueTask OpenWriteAsync(
			string fileName,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Writes a header row with specified names.
		/// </summary>
		/// <param name="columns">the names of the data columns</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of writing a header row</returns>
		ValueTask WriteHeaderRowAsync(
			string[] columns,
			CancellationToken cancellationToken = default);

		/// <summary>
		/// Writes a data row with specified values.
		/// </summary>
		/// <param name="values">the values in the row</param>
		/// <param name="cancellationToken">the cancellation token</param>
		/// <returns>the task of writing a data row</returns>
		ValueTask WriteDataRowAsync(
			object[] values, 
			CancellationToken cancellationToken = default);
	}
}
