using System;
using System.Globalization;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Provides a base type for export providers.
	/// </summary>
	public abstract class ExportProviderBase : IExportProvider
	{
		#region IExportProvider Members

		/// <summary>
		/// Gets or sets the culture to use when exporting values.
		/// </summary>
		/// <remarks>
		/// The default is <see cref="CultureInfo.InvariantCulture"/>.
		/// </remarks>
		public CultureInfo Culture { get; set; } = CultureInfo.InvariantCulture;

		/// <inheritdoc/>
		public abstract void OpenWrite(string filePath);

		/// <inheritdoc/>
		public abstract void WriteHeaderRow(string[] columns);

		/// <inheritdoc/>
		public abstract void WriteDataRow(object[] values);

		#endregion

		#region IDisposable Members

		/// <inheritdoc/>
		public void Dispose()
		{
			Dispose(true);

			GC.SuppressFinalize(this);
		}

		#endregion

		/// <summary>
		/// Disposes any unmanaged resources allocated by this object.
		/// </summary>
		~ExportProviderBase()
		{
			Dispose(false);
		}

		/// <summary>
		/// Releases any resources allocated by this object.
		/// </summary>
		/// <param name="disposing">the flag indicating whether the object is being disposed, or the garbage collector has kicked in</param>
		protected virtual void Dispose(bool disposing)
		{
		}
	}
}
