namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an export provider for writing a comma-delimited CSV file.
	/// </summary>
	public class CommaCsvExportProvider : CsvExportProviderBase
	{
		#region ExportProvider Members

		/// <inheritdoc/>
		public override string Name
			=> "CSV (Comma-Separated Values)";

		/// <inheritdoc/>
		public override string DefaultFileExtension
			=> ".csv";

		#endregion

		#region CsvExportProviderBase Members

		/// <inheritdoc/>
		protected override string Delimiter
			=> ",";

		#endregion
	}
}
