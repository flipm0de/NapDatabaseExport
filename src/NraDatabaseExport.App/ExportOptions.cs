using NraDatabaseExport.DbProviders;
using NraDatabaseExport.ExportProviders;

namespace NraDatabaseExport.App
{
	/// <summary>
	/// Represents the export options.
	/// </summary>
	public class ExportOptions
	{
		/// <summary>
		/// Gets or sets the database provider type.
		/// </summary>
		public DbProviderType? DbProviderType { get; set; }

		/// <summary>
		/// Gets or sets the database file name.
		/// </summary>
		public string? DatabaseFileName { get; set; }

		/// <summary>
		/// Gets or sets the server name.
		/// </summary>
		public string? ServerName { get; set; }

		/// <summary>
		/// Gets or sets the user name.
		/// </summary>
		public string? UserName { get; set; }

		/// <summary>
		/// Gets or sets the password.
		/// </summary>
		public string? Password { get; set; }

		/// <summary>
		/// Gets or sets the database.
		/// </summary>
		public string? Database { get; set; }

		/// <summary>
		/// Gets or sets the array of tables.
		/// </summary>
		public string[]? Tables { get; set; }

		/// <summary>
		/// Gets or sets the export provider type.
		/// </summary>
		public ExportProviderType? ExportProviderType { get; set; }
	}
}
