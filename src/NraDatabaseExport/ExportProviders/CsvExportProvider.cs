#if SUPPORT_CSV
namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Represents an export provider for writing a CSV file.
	/// </summary>
	[ExportProvider(ExportProviderType.Csv, "CSV",
		DefaultExtension = ".csv")]
	public class CsvExportProvider : CsvExportProviderBase
	{
	}
}
#endif
