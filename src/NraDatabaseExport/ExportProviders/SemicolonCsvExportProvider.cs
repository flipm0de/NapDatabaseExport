namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an export provider for writing a semicolon-delimited CSV file.
	/// </summary>
	public class SemicolonCsvExportProvider : CsvExportProviderBase
	{
		#region ExportProvider Members

		/// <inheritdoc/>
		public override string Name
			=> "CSV (Semicolon-Separated Values)";

		/// <inheritdoc/>
		public override string DefaultFileExtension
			=> ".csv";

		#endregion

		#region CsvExportProviderBase Members

		/// <inheritdoc/>
		protected override string Delimiter
			=> ";";

		#endregion
	}
}
