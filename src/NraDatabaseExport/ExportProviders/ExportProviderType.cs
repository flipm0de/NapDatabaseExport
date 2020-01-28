using System.Globalization;

namespace NraDatabaseExport.ExportProviders
{
	/// <summary>
	/// Provides an enumeration of export providers.
	/// </summary>
	public enum ExportProviderType
	{
		/// <summary>
		/// JSON
		/// </summary>
		/// <remarks>
		/// Rows are written as arrays.
		/// </remarks>
		Json,

		/// <summary>
		/// CSV
		/// </summary>
		/// <remarks>
		/// Delimiter is inferred from <see cref="CultureInfo.CurrentCulture"/>
		/// </remarks>
		Csv,

		/// <summary>
		/// Comma-separated CSV
		/// </summary>
		CommaCsv,

		/// <summary>
		/// Semicolon-separated CSV
		/// </summary>
		SemicolonCsv,
	}
}
