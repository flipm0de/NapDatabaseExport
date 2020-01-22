namespace NraDatabaseExport.ExportProviders
{
	public class SemicolonCsvExportProvider : CsvExportProviderBase
	{
		#region ExportProvider Members

		/// <inheritdoc/>
		public override string ExportType
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
