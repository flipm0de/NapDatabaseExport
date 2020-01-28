using System;
using System.Globalization;

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
		/// Begins writing the data into a file with a specified name.
		/// </summary>
		/// <param name="fileName">the name of the file to export data to</param>
		void OpenWrite(string fileName);

		/// <summary>
		/// Writes a header row containing the names of the data columns.
		/// </summary>
		/// <param name="columns">the names of the data columns</param>
		void WriteHeaderRow(string[] columns);

		/// <summary>
		/// Writes a data row containing the values in the row.
		/// </summary>
		/// <param name="values"></param>
		void WriteDataRow(object[] values);
	}
}
