namespace NraDatabaseExport.ExportProviders
{
	public class CommaCsvExportProvider : CsvExportProviderBase
	{
		#region ExportProvider Members

		/// <inheritdoc/>
		public override string ExportType
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
