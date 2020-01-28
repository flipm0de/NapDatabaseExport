#if SUPPORT_CSV
namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an export provider for writing a semicolon-separated CSV file.
	/// </summary>
	[ExportProvider(ExportProviderType.SemicolonCsv, "Semicolon-separated CSV",
		DefaultExtension = ".csv")]
	public class SemicolonCsvExportProvider : CsvExportProviderBase
	{
		#region CsvExportProviderBase Members

		/// <inheritdoc/>
		protected override string Delimiter
			=> ";";

		#endregion
	}
}
#endif
