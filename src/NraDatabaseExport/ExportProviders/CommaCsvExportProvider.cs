#if SUPPORT_CSV
namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an export provider for writing a comma-separated CSV file.
	/// </summary>
	[ExportProvider(ExportProviderType.CommaCsv, "Comma-separated CSV",
		DefaultExtension = ".csv")]
	public class CommaCsvExportProvider : CsvExportProviderBase
	{
		#region CsvExportProviderBase Members

		/// <inheritdoc/>
		protected override string Delimiter
			=> ",";

		#endregion
	}
}
#endif
